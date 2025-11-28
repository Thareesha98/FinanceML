using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceML.Core.Models
{
    /// <summary>
    /// Represents customizable application preferences for a specific user.
    /// Includes UI theme, currency preferences, backup settings, and notification control.
    /// Designed for clarity, validation, and future microservice expansion.
    /// </summary>
    public class AppSettings
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The associated user that owns these settings.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Selected UI theme. Stored as string for easy serialization, 
        /// but strongly recommended to use <see cref="AppTheme"/> for business logic.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Theme { get; set; } = AppTheme.Light.ToString();

        /// <summary>
        /// Full currency label used in UI (e.g., "LKR (Rs)", "USD ($)")
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Currency { get; set; } = CurrencyDefaults.DefaultCurrencyLabel;

        /// <summary>
        /// Symbol for UI display (e.g., "Rs", "$", "€")
        /// </summary>
        [Required]
        [MaxLength(5)]
        public string CurrencySymbol { get; set; } = CurrencyDefaults.DefaultSymbol;

        /// <summary>
        /// Enables or disables notifications in the application.
        /// </summary>
        public bool NotificationsEnabled { get; set; } = true;

        /// <summary>
        /// Enables auto-backup functionality. (Future feature: Sync with cloud)
        /// </summary>
        public bool AutoBackup { get; set; } = false;

        /// <summary>
        /// Timestamp for record creation.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp for last update.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Utility to update timestamp automatically.
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // ================================================================
    // ENUMS
    // ================================================================

    /// <summary>
    /// Defines available UI themes.
    /// </summary>
    public enum AppTheme
    {
        Light,
        Dark,
        Auto
    }

    // ================================================================
    // CURRENCY DEFAULTS & UTILITIES
    // ================================================================

    public static class CurrencyDefaults
    {
        public const string DefaultCurrencyLabel = "LKR (Rs)";
        public const string DefaultSymbol = "Rs";
    }

    /// <summary>
    /// Utility class responsible for resolving currency symbols and ISO codes.
    /// Centralized for maintainability and easier future extension.
    /// </summary>
    public static class CurrencyHelper
    {
        /// <summary>
        /// Returns only the symbol component of the currency.
        /// </summary>
        public static string GetCurrencySymbol(string currencyLabel)
        {
            return currencyLabel switch
            {
                "LKR (Rs)" => "Rs",
                "USD ($)" => "$",
                "EUR (€)" => "€",
                "GBP (£)" => "£",
                "JPY (¥)" => "¥",
                "CAD ($)" => "C$",
                "AUD ($)" => "A$",
                _ => CurrencyDefaults.DefaultSymbol
            };
        }

        /// <summary>
        /// Returns standardized ISO currency code.
        /// </summary>
        public static string GetCurrencyCode(string currencyLabel)
        {
            return currencyLabel switch
            {
                "LKR (Rs)" => "LKR",
                "USD ($)" => "USD",
                "EUR (€)" => "EUR",
                "GBP (£)" => "GBP",
                "JPY (¥)" => "JPY",
                "CAD ($)" => "CAD",
                "AUD ($)" => "AUD",
                _ => "LKR"
            };
        }
    }
}

