using System;

namespace FinanceML.Core.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Theme { get; set; } = "Light";
        public string Currency { get; set; } = "LKR (Rs)";
        public string CurrencySymbol { get; set; } = "Rs";
        public bool NotificationsEnabled { get; set; } = true;
        public bool AutoBackup { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum AppTheme
    {
        Light,
        Dark,
        Auto
    }

    public static class CurrencyHelper
    {
        public static string GetCurrencySymbol(string currency)
        {
            return currency switch
            {
                "LKR (Rs)" => "Rs",
                "USD ($)" => "$",
                "EUR (€)" => "€",
                "GBP (£)" => "£",
                "JPY (¥)" => "¥",
                "CAD ($)" => "C$",
                "AUD ($)" => "A$",
                _ => "Rs"
            };
        }

        public static string GetCurrencyCode(string currency)
        {
            return currency switch
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
