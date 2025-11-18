using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Repositories;
using FinanceML.Core.Utilities;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// Provides user settings management with caching, async operations,
    /// dependency injection, and change notifications.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;

        // Thread-safe cache for currently loaded settings
        private AppSettings? _currentSettings;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public AppSettings? CurrentSettings => _currentSettings;

        // Event fired when settings change
        public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

        public SettingsService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository 
                ?? throw new ArgumentNullException(nameof(settingsRepository));
        }

        // ==========================================================
        // LOAD SETTINGS (ASYNC)
        // ==========================================================
        public async Task<Result<AppSettings>> LoadUserSettingsAsync(
            int userId, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                var settings = await _settingsRepository.GetOrCreateUserSettingsAsync(userId, cancellationToken);

                _currentSettings = settings;
                return Result<AppSettings>.Success(settings);
            }
            catch (Exception ex)
            {
                return Result<AppSettings>.Failure($"Failed to load settings: {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }
        }

        // ==========================================================
        // SAVE SETTINGS (ASYNC)
        // ==========================================================
        public async Task<Result> SaveSettingsAsync(
            AppSettings newSettings, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_currentSettings == null)
                    return Result.Failure("Settings not loaded.");

                var oldSettings = CloneSettings(_currentSettings);

                // Update settings
                _currentSettings.Theme = newSettings.Theme;
                _currentSettings.Currency = newSettings.Currency;
                _currentSettings.CurrencySymbol = CurrencyHelper.GetCurrencySymbol(newSettings.Currency);
                _currentSettings.NotificationsEnabled = newSettings.NotificationsEnabled;
                _currentSettings.AutoBackup = newSettings.AutoBackup;
                _currentSettings.UpdatedAt = DateTime.UtcNow;

                var ok = await _settingsRepository.UpdateUserSettingsAsync(_currentSettings, cancellationToken);

                if (!ok) 
                    return Result.Failure("Could not save settings.");

                // Notify subscribers
                SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
                {
                    OldSettings = oldSettings,
                    NewSettings = CloneSettings(_currentSettings)
                });

                return Result.Success();
            }
            finally
            {
                _lock.Release();
            }
        }

        // ==========================================================
        // READ-ONLY ACCESSORS
        // ==========================================================
        public string GetCurrentTheme() => _currentSettings?.Theme ?? "Light";

        public string GetCurrentCurrency() => _currentSettings?.Currency ?? "LKR (Rs)";

        public string GetCurrentCurrencySymbol() => _currentSettings?.CurrencySymbol ?? "Rs";

        public bool IsNotificationsEnabled() => _currentSettings?.NotificationsEnabled ?? true;

        public bool IsAutoBackupEnabled() => _currentSettings?.AutoBackup ?? false;

        // ==========================================================
        // UTILITIES
        // ==========================================================
        private AppSettings CloneSettings(AppSettings s) =>
            new AppSettings
            {
                Theme = s.Theme,
                Currency = s.Currency,
                CurrencySymbol = s.CurrencySymbol,
                NotificationsEnabled = s.NotificationsEnabled,
                AutoBackup = s.AutoBackup,
                UpdatedAt = s.UpdatedAt
            };
    }

    // ==========================================================
    // EVENTS
    // ==========================================================
    public class SettingsChangedEventArgs : EventArgs
    {
        public AppSettings? OldSettings { get; set; }
        public AppSettings? NewSettings { get; set; }
    }
}

