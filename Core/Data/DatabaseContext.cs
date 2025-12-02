using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FinanceML.Core.Data;

public sealed class DatabaseContext : IDisposable
{
    private SQLiteConnection? _connection;
    private bool _disposed;
    private readonly string _connectionString;

    public DatabaseContext()
    {
        string appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FinanceML"
        );

        Directory.CreateDirectory(appFolder);

        string dbPath = Path.Combine(appFolder, "finance.db");
        _connectionString = $"Data Source={dbPath};Version=3;Foreign Keys=True;";

        InitializeDatabase();
    }

    /// <summary>
    /// Provides a lazily-created, open SQLite connection.
    /// </summary>
    public SQLiteConnection GetConnection()
    {
        if (_connection is { State: ConnectionState.Open })
            return _connection;

        _connection = new SQLiteConnection(_connectionString);
        _connection.Open();
        return _connection;
    }

    /// <summary>
    /// Ensures required tables + indexes exist.
    /// </summary>
    private void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using var command = new SQLiteCommand(connection);

        foreach (var sql in DatabaseSchema.AllScripts)
        {
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _connection?.Dispose();
        }
        _disposed = true;
    }
}

/// <summary>
/// Holds all SQL creation scripts in one structured location.
/// Easy to maintain and version.
/// </summary>
internal static class DatabaseSchema
{
    public static readonly string CreateUsers = @"
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
        );
    ";

    public static readonly string CreateTransactions = @"
        CREATE TABLE IF NOT EXISTS Transactions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            Date DATETIME NOT NULL,
            Description TEXT NOT NULL,
            Category TEXT NOT NULL,
            Amount DECIMAL(10,2) NOT NULL,
            Type INTEGER NOT NULL,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        );
    ";

    public static readonly string CreateBudgets = @"
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
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        );
    ";

    public static readonly string CreateSettings = @"
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
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        );
    ";

    public static readonly string CreateIndexes = @"
        CREATE INDEX IF NOT EXISTS idx_trans_user ON Transactions(UserId);
        CREATE INDEX IF NOT EXISTS idx_trans_date ON Transactions(Date);
        CREATE INDEX IF NOT EXISTS idx_trans_cat ON Transactions(Category);

        CREATE INDEX IF NOT EXISTS idx_budget_user ON Budgets(UserId);
        CREATE INDEX IF NOT EXISTS idx_budget_cat ON Budgets(Category);

        CREATE INDEX IF NOT EXISTS idx_settings_user ON Settings(UserId);
    ";

    /// <summary>
    /// Stored in execution order.
    /// </summary>
    public static readonly string[] AllScripts =
    {
        CreateUsers,
        CreateTransactions,
        CreateBudgets,
        CreateSettings,
        CreateIndexes
    };
}

