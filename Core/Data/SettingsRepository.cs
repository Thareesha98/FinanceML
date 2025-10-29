using System;
using System.Data.SQLite;
using FinanceML.Core.Models;

namespace FinanceML.Core.Data
{
    public class SettingsRepository
    {
        private readonly DatabaseContext _context;

        public SettingsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public AppSettings? GetUserSettings(int userId)
        {
            const string sql = "SELECT * FROM Settings WHERE UserId = @UserId";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new AppSettings
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Theme = reader["Theme"].ToString() ?? "Light",
                    Currency = reader["Currency"].ToString() ?? "LKR (Rs)",
                    CurrencySymbol = reader["CurrencySymbol"].ToString() ?? "Rs",
                    NotificationsEnabled = Convert.ToBoolean(reader["NotificationsEnabled"]),
                    AutoBackup = Convert.ToBoolean(reader["AutoBackup"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };
            }

            return null;
        }

        public int CreateUserSettings(AppSettings settings)
        {
            const string sql = @"
                INSERT INTO Settings (UserId, Theme, Currency, CurrencySymbol, NotificationsEnabled, AutoBackup, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Theme, @Currency, @CurrencySymbol, @NotificationsEnabled, @AutoBackup, @CreatedAt, @UpdatedAt);
                SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", settings.UserId);
            command.Parameters.AddWithValue("@Theme", settings.Theme);
            command.Parameters.AddWithValue("@Currency", settings.Currency);
            command.Parameters.AddWithValue("@CurrencySymbol", settings.CurrencySymbol);
            command.Parameters.AddWithValue("@NotificationsEnabled", settings.NotificationsEnabled);
            command.Parameters.AddWithValue("@AutoBackup", settings.AutoBackup);
            command.Parameters.AddWithValue("@CreatedAt", settings.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", settings.UpdatedAt);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool UpdateUserSettings(AppSettings settings)
        {
            const string sql = @"
                UPDATE Settings 
                SET Theme = @Theme, Currency = @Currency, CurrencySymbol = @CurrencySymbol, 
                    NotificationsEnabled = @NotificationsEnabled, AutoBackup = @AutoBackup, UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", settings.UserId);
            command.Parameters.AddWithValue("@Theme", settings.Theme);
            command.Parameters.AddWithValue("@Currency", settings.Currency);
            command.Parameters.AddWithValue("@CurrencySymbol", settings.CurrencySymbol);
            command.Parameters.AddWithValue("@NotificationsEnabled", settings.NotificationsEnabled);
            command.Parameters.AddWithValue("@AutoBackup", settings.AutoBackup);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            return command.ExecuteNonQuery() > 0;
        }

        public AppSettings GetOrCreateUserSettings(int userId)
        {
            var settings = GetUserSettings(userId);
            if (settings == null)
            {
                // Create default settings for the user
                settings = new AppSettings
                {
                    UserId = userId,
                    Theme = "Light",
                    Currency = "LKR (Rs)",
                    CurrencySymbol = "Rs",
                    NotificationsEnabled = true,
                    AutoBackup = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var id = CreateUserSettings(settings);
                settings.Id = id;
            }

            return settings;
        }
    }
}
