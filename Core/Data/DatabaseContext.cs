using System;
using System.Data.SQLite;
using System.IO;

namespace FinanceML.Core.Data
{
    public class DatabaseContext : IDisposable
    {
        private SQLiteConnection? _connection;
        private readonly string _connectionString;

        public DatabaseContext()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "FinanceML");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            var dbPath = Path.Combine(appFolder, "finance.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            
            InitializeDatabase();
        }

        public SQLiteConnection GetConnection()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _connection = new SQLiteConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // Create Users table
            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    Email TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    FirstName TEXT,
                    LastName TEXT,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastLoginDate DATETIME,
                    IsActive BOOLEAN DEFAULT 1
                );";

            // Create Transactions table
            var createTransactionsTable = @"
                CREATE TABLE IF NOT EXISTS Transactions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Date DATETIME NOT NULL,
                    Description TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Amount DECIMAL(10,2) NOT NULL,
                    Type INTEGER NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );";

            // Create Budgets table
            var createBudgetsTable = @"
                CREATE TABLE IF NOT EXISTS Budgets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Amount DECIMAL(10,2) NOT NULL,
                    Period INTEGER NOT NULL,
                    StartDate DATETIME NOT NULL,
                    EndDate DATETIME NOT NULL,
                    SpentAmount DECIMAL(10,2) DEFAULT 0,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    IsActive BOOLEAN DEFAULT 1,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );";

            // Create Settings table
            var createSettingsTable = @"
                CREATE TABLE IF NOT EXISTS Settings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Theme TEXT DEFAULT 'Light',
                    Currency TEXT DEFAULT 'LKR (Rs)',
                    CurrencySymbol TEXT DEFAULT 'Rs',
                    NotificationsEnabled BOOLEAN DEFAULT 1,
                    AutoBackup BOOLEAN DEFAULT 0,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );";

            // Create indexes for better performance
            var createIndexes = @"
                CREATE INDEX IF NOT EXISTS idx_transactions_userid ON Transactions(UserId);
                CREATE INDEX IF NOT EXISTS idx_transactions_date ON Transactions(Date);
                CREATE INDEX IF NOT EXISTS idx_transactions_category ON Transactions(Category);
                CREATE INDEX IF NOT EXISTS idx_budgets_userid ON Budgets(UserId);
                CREATE INDEX IF NOT EXISTS idx_budgets_category ON Budgets(Category);
                CREATE INDEX IF NOT EXISTS idx_settings_userid ON Settings(UserId);";

            using var command = new SQLiteCommand(createUsersTable, connection);
            command.ExecuteNonQuery();

            command.CommandText = createTransactionsTable;
            command.ExecuteNonQuery();

            command.CommandText = createBudgetsTable;
            command.ExecuteNonQuery();

            command.CommandText = createSettingsTable;
            command.ExecuteNonQuery();

            command.CommandText = createIndexes;
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
