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
    /// concurrency protection, DI, and event notifications.
    /// Extensively modularized to maximize maintainability + commit granularity.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepo;

        // Thread-safe in-memory cache
        private AppSettings? _cached;
        private readonly SemaphoreSlim _lock = new(1, 1);

        // Event fired on change
        public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

        public SettingsService(ISettingsRepository repo)
        {
            _settingsRepo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        // ==========================================================
        // PUBLIC PROPERTIES (cached reads)
        // ==========================================================

        public AppSettings? CurrentSettings => _cached;

        public string CurrentTheme => _cached?.Theme ?? "Light";
        public string CurrentCurrency => _cached?.Currency ?? "LKR (Rs)";
        public string CurrentSymbol => _cached?.CurrencySymbol ?? "Rs";
        public bool NotificationsEnabled => _cached?.NotificationsEnabled ?? true;
        public bool AutoBackupEnabled => _cached?.AutoBackup ?? false;

        // ==========================================================
        // LOAD SETTINGS
        // ==========================================================

        public async Task<Result<AppSettings>> LoadUserSettingsAsync(
            int userId,
            CancellationToken token = default)
        {
            await _lock.WaitAsync(token);
            try
            {
                var settings = await _settingsRepo.GetOrCreateUserSettingsAsync(userId, token);
                _cached = settings;

                return Result<AppSettings>.Success(Clone(settings));
            }
            catch (Exception ex)
            {
                return Result<AppSettings>.Failure($"Failed to load user settings: {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }
        }

        // ==========================================================
        // SAVE SETTINGS
        // ==========================================================

        public async Task<Result> SaveSettingsAsync(
            AppSettings newSettings,
            CancellationToken token = default)
        {
            await _lock.WaitAsync(token);
            try
            {
                if (_cached is null)
                    return Result.Failure("Settings must be loaded before saving.");

                var old = Clone(_cached);

                ApplyUpdates(_cached, newSettings);

                var ok = await _settingsRepo.UpdateUserSettingsAsync(_cached, token);
                if (!ok)
                    return Result.Failure("Failed to persist updated settings");

                RaiseChangedEvent(old, _cached);
                return Result.Success();
            }
            finally
            {
                _lock.Release();
            }
        }

        // ==========================================================
        // INTERNAL UPDATE LOGIC
        // ==========================================================

        private void ApplyUpdates(AppSettings current, AppSettings incoming)
        {
            current.Theme = incoming.Theme;
            current.Currency = incoming.Currency;
            current.CurrencySymbol = CurrencyHelper.GetCurrencySymbol(incoming.Currency);
            current.NotificationsEnabled = incoming.NotificationsEnabled;
            current.AutoBackup = incoming.AutoBackup;
            current.UpdatedAt = DateTime.UtcNow;
        }

        // ==========================================================
        // EVENT INVOCATION (Extracted for cleaner commits)
        // ==========================================================

        private void RaiseChangedEvent(AppSettings oldValues, AppSettings newValues)
        {
            SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
            {
                OldSettings = Clone(oldValues),
                NewSettings = Clone(newValues)
            });
        }
        
	 private void RaiseChangedSystemEvenets(AppSettings oldValues, AppSettings newValues)
        {
            SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
            {
                OldSettings = Clone(oldValues),
                NewSettings = Clone(newValues)
            });
        }

        // ==========================================================
        // CLONE (Safely returns new instances)
        // ==========================================================

        private AppSettings Clone(AppSettings s)
        {
            return new AppSettings
            {
                Theme = s.Theme,
                Currency = s.Currency,
                CurrencySymbol = s.CurrencySymbol,
                NotificationsEnabled = s.NotificationsEnabled,
                AutoBackup = s.AutoBackup,
                UpdatedAt = s.UpdatedAt
            };
        }
    }

    // ==========================================================
    // SETTINGS EVENT PAYLOAD
    // ==========================================================

    public class SettingsChangedEventArgs : EventArgs
    {
        public AppSettings? OldSettings { get; set; }
        public AppSettings? NewSettings { get; set; }
    }
}

