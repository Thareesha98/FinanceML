using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FinanceML.Core.Models;

namespace FinanceML.Core.Data
{
    public class BudgetRepository
    {
        private readonly DatabaseContext _context;

        public BudgetRepository(DatabaseContext context)
        {
            _context = context;
        }

        public int CreateBudget(Budget budget, int userId)
        {
            const string sql = @"
                INSERT INTO Budgets (UserId, Name, Category, Amount, Period, StartDate, EndDate, SpentAmount, CreatedAt, IsActive)
                VALUES (@UserId, @Name, @Category, @Amount, @Period, @StartDate, @EndDate, @SpentAmount, @CreatedAt, @IsActive);
                SELECT last_insert_rowid();";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Name", budget.Name);
            command.Parameters.AddWithValue("@Category", budget.Category);
            command.Parameters.AddWithValue("@Amount", budget.Amount);
            command.Parameters.AddWithValue("@Period", (int)budget.Period);
            command.Parameters.AddWithValue("@StartDate", budget.StartDate);
            command.Parameters.AddWithValue("@EndDate", budget.EndDate);
            command.Parameters.AddWithValue("@SpentAmount", budget.SpentAmount);
            command.Parameters.AddWithValue("@CreatedAt", budget.CreatedAt);
            command.Parameters.AddWithValue("@IsActive", budget.IsActive);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<Budget> GetAllBudgets(int userId)
        {
            const string sql = "SELECT * FROM Budgets WHERE UserId = @UserId AND IsActive = 1 ORDER BY Category";
            var budgets = new List<Budget>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                budgets.Add(new Budget
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Period = (BudgetPeriod)Convert.ToInt32(reader["Period"]),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    SpentAmount = Convert.ToDecimal(reader["SpentAmount"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return budgets;
        }

        public Budget? GetBudgetById(int id)
        {
            const string sql = "SELECT * FROM Budgets WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Budget
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Period = (BudgetPeriod)Convert.ToInt32(reader["Period"]),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    SpentAmount = Convert.ToDecimal(reader["SpentAmount"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return null;
        }

        public bool UpdateBudget(Budget budget)
        {
            const string sql = @"
                UPDATE Budgets 
                SET Name = @Name, Category = @Category, Amount = @Amount, Period = @Period,
                    StartDate = @StartDate, EndDate = @EndDate, SpentAmount = @SpentAmount, IsActive = @IsActive
                WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", budget.Id);
            command.Parameters.AddWithValue("@Name", budget.Name);
            command.Parameters.AddWithValue("@Category", budget.Category);
            command.Parameters.AddWithValue("@Amount", budget.Amount);
            command.Parameters.AddWithValue("@Period", (int)budget.Period);
            command.Parameters.AddWithValue("@StartDate", budget.StartDate);
            command.Parameters.AddWithValue("@EndDate", budget.EndDate);
            command.Parameters.AddWithValue("@SpentAmount", budget.SpentAmount);
            command.Parameters.AddWithValue("@IsActive", budget.IsActive);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteBudget(int id)
        {
            const string sql = "UPDATE Budgets SET IsActive = 0 WHERE Id = @Id";

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public void RecalculateBudgetSpentAmounts(int userId)
        {
            // Get all active budgets for the user
            var budgets = GetAllBudgets(userId);

            foreach (var budget in budgets)
            {
                // Calculate spent amount for this budget based on transactions in the budget period
                const string sql = @"
                    SELECT COALESCE(SUM(Amount), 0) 
                    FROM Transactions 
                    WHERE UserId = @UserId 
                    AND Category = @Category 
                    AND Type = @Type 
                    AND Date >= @StartDate 
                    AND Date <= @EndDate";

                using var command = new SQLiteCommand(sql, _context.GetConnection());
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Category", budget.Category);
                command.Parameters.AddWithValue("@Type", (int)TransactionType.Expense);
                command.Parameters.AddWithValue("@StartDate", budget.StartDate);
                command.Parameters.AddWithValue("@EndDate", budget.EndDate);

                var spentAmount = Convert.ToDecimal(command.ExecuteScalar());

                // Update the budget's spent amount
                const string updateSql = "UPDATE Budgets SET SpentAmount = @SpentAmount WHERE Id = @Id";
                using var updateCommand = new SQLiteCommand(updateSql, _context.GetConnection());
                updateCommand.Parameters.AddWithValue("@SpentAmount", spentAmount);
                updateCommand.Parameters.AddWithValue("@Id", budget.Id);
                updateCommand.ExecuteNonQuery();
            }
        }

        public List<Budget> GetBudgetsByCategory(int userId, string category)
        {
            const string sql = "SELECT * FROM Budgets WHERE UserId = @UserId AND Category = @Category AND IsActive = 1";
            var budgets = new List<Budget>();

            using var command = new SQLiteCommand(sql, _context.GetConnection());
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Category", category);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                budgets.Add(new Budget
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Category = reader["Category"].ToString() ?? "",
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Period = (BudgetPeriod)Convert.ToInt32(reader["Period"]),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    SpentAmount = Convert.ToDecimal(reader["SpentAmount"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return budgets;
        }
    }
}
