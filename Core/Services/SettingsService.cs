using System;
using FinanceML.Core.Models;
using FinanceML.Core.Data;

namespace FinanceML.Core.Services
{
    public class SettingsService : IDisposable
    {
        private static SettingsService? _instance;
        private readonly DatabaseContext _context;
        private readonly SettingsRepository _settingsRepository;
        private AppSettings? _currentSettings;

        public static SettingsService Instance => _instance ??= new SettingsService();

        public AppSettings? CurrentSettings => _currentSettings;

        // Events for settings changes
        public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

        private SettingsService()
        {
            _context = new DatabaseContext();
            _settingsRepository = new SettingsRepository(_context);
        }

        public void LoadUserSettings(int userId)
        {
            _currentSettings = _settingsRepository.GetOrCreateUserSettings(userId);
        }

        public bool SaveSettings(AppSettings settings)
        {
            if (_currentSettings == null) return false;

            var oldSettings = new AppSettings
            {
                Theme = _currentSettings.Theme,
                Currency = _currentSettings.Currency,
                CurrencySymbol = _currentSettings.CurrencySymbol
            };

            _currentSettings.Theme = settings.Theme;
            _currentSettings.Currency = settings.Currency;
            _currentSettings.CurrencySymbol = CurrencyHelper.GetCurrencySymbol(settings.Currency);
            _currentSettings.NotificationsEnabled = settings.NotificationsEnabled;
            _currentSettings.AutoBackup = settings.AutoBackup;
            _currentSettings.UpdatedAt = DateTime.Now;

            var success = _settingsRepository.UpdateUserSettings(_currentSettings);

            if (success)
            {
                // Notify about settings changes
                SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
                {
                    OldSettings = oldSettings,
                    NewSettings = _currentSettings
                });
            }

            return success;
        }

        public string GetCurrentTheme()
        {
            return _currentSettings?.Theme ?? "Light";
        }

        public string GetCurrentCurrency()
        {
            return _currentSettings?.Currency ?? "LKR (Rs)";
        }

        public string GetCurrentCurrencySymbol()
        {
            return _currentSettings?.CurrencySymbol ?? "Rs";
        }

        public bool IsNotificationsEnabled()
        {
            return _currentSettings?.NotificationsEnabled ?? true;
        }

        public bool IsAutoBackupEnabled()
        {
            return _currentSettings?.AutoBackup ?? false;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class SettingsChangedEventArgs : EventArgs
    {
        public AppSettings? OldSettings { get; set; }
        public AppSettings? NewSettings { get; set; }
    }
}
