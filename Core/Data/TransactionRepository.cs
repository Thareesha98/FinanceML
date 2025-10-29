using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using FinanceML.Core.Models;

namespace FinanceML.Core.Data
{
    public class TransactionRepository
    {
        private readonly DatabaseContext _context;

        public TransactionRepository(DatabaseContext context)
        {
            _context = context;
        }

        public int CreateTransaction(Transaction transaction, int userId)
        {
            const string sql = @"
                INSERT INTO Transactions (UserId, Date, Description, Category, Amount, Type, CreatedAt)
                VALUES (@UserId, @Date, @Description, @Category, @Amount, @Type, @CreatedAt);
                SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Date", transaction.Date);
            command.Parameters.AddWithValue("@Description", transaction.Description);
            command.Parameters.AddWithValue("@Category", transaction.Category);
            command.Parameters.AddWithValue("@Amount", transaction.Amount);
            command.Parameters.AddWithValue("@Type", (int)transaction.Type);
            command.Parameters.AddWithValue("@CreatedAt", transaction.CreatedAt);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Transaction> GetAllTransactions(int userId)
        {
            const string sql = "SELECT * FROM Transactions WHERE UserId = @UserId ORDER BY Date DESC";
            var transactions = new List<Transaction>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Description = reader["Description"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Type = (TransactionType)Convert.ToInt32(reader["Type"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return transactions;
        }

        public Transaction? GetTransactionById(int id)
        {
            const string sql = "SELECT * FROM Transactions WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Transaction
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Description = reader["Description"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Type = (TransactionType)Convert.ToInt32(reader["Type"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }

            return null;
        }

        public bool UpdateTransaction(Transaction transaction)
        {
            const string sql = @"
                UPDATE Transactions 
                SET Date = @Date, Description = @Description, Category = @Category, 
                    Amount = @Amount, Type = @Type
                WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", transaction.Id);
            command.Parameters.AddWithValue("@Date", transaction.Date);
            command.Parameters.AddWithValue("@Description", transaction.Description);
            command.Parameters.AddWithValue("@Category", transaction.Category);
            command.Parameters.AddWithValue("@Amount", transaction.Amount);
            command.Parameters.AddWithValue("@Type", (int)transaction.Type);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteTransaction(int id)
        {
            const string sql = "DELETE FROM Transactions WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public List<Transaction> GetTransactionsByDateRange(int userId, DateTime? startDate, DateTime? endDate)
        {
            var sql = "SELECT * FROM Transactions WHERE UserId = @UserId";
            var parameters = new List<SQLiteParameter> { new("@UserId", userId) };

            if (startDate.HasValue)
            {
                sql += " AND Date >= @StartDate";
                parameters.Add(new SQLiteParameter("@StartDate", startDate.Value));
            }

            if (endDate.HasValue)
            {
                sql += " AND Date <= @EndDate";
                parameters.Add(new SQLiteParameter("@EndDate", endDate.Value));
            }

            sql += " ORDER BY Date DESC";

            var transactions = new List<Transaction>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            foreach (var param in parameters)
            {
                command.Parameters.Add(param);
            }

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Description = reader["Description"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Type = (TransactionType)Convert.ToInt32(reader["Type"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return transactions;
        }

        public List<Transaction> GetTransactionsByCategory(int userId, string category)
        {
            const string sql = "SELECT * FROM Transactions WHERE UserId = @UserId AND Category = @Category ORDER BY Date DESC";
            var transactions = new List<Transaction>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Category", category);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Description = reader["Description"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Type = (TransactionType)Convert.ToInt32(reader["Type"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return transactions;
        }

        public decimal GetTotalIncome(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sql = "SELECT COALESCE(SUM(Amount), 0) FROM Transactions WHERE UserId = @UserId AND Type = @Type";
            var parameters = new List<SQLiteParameter> 
            { 
                new("@UserId", userId),
                new("@Type", (int)TransactionType.Income)
            };

            if (startDate.HasValue)
            {
                sql += " AND Date >= @StartDate";
                parameters.Add(new SQLiteParameter("@StartDate", startDate.Value));
            }

            if (endDate.HasValue)
            {
                sql += " AND Date <= @EndDate";
                parameters.Add(new SQLiteParameter("@EndDate", endDate.Value));
            }

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            foreach (var param in parameters)
            {
                command.Parameters.Add(param);
            }

            return Convert.ToDecimal(command.ExecuteScalar());
        }

        public decimal GetTotalExpenses(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sql = "SELECT COALESCE(SUM(Amount), 0) FROM Transactions WHERE UserId = @UserId AND Type = @Type";
            var parameters = new List<SQLiteParameter> 
            { 
                new("@UserId", userId),
                new("@Type", (int)TransactionType.Expense)
            };

            if (startDate.HasValue)
            {
                sql += " AND Date >= @StartDate";
                parameters.Add(new SQLiteParameter("@StartDate", startDate.Value));
            }

            if (endDate.HasValue)
            {
                sql += " AND Date <= @EndDate";
                parameters.Add(new SQLiteParameter("@EndDate", endDate.Value));
            }

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            foreach (var param in parameters)
            {
                command.Parameters.Add(param);
            }

            return Convert.ToDecimal(command.ExecuteScalar());
        }



        public Dictionary<string, decimal> GetMonthlyExpensesByCategory(int userId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT Category, SUM(Amount) as TotalAmount
                FROM Transactions 
                WHERE UserId = @UserId AND Type = @Type AND Date >= @StartDate AND Date <= @EndDate
                GROUP BY Category";

            var expenses = new Dictionary<string, decimal>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Type", (int)TransactionType.Expense);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var category = reader["Category"].ToString() ?? "";
                var amount = Convert.ToDecimal(reader["TotalAmount"]);
                expenses[category] = amount;
            }

            return expenses;
        }
    }
}
