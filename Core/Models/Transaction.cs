using System;

namespace FinanceML.Core.Models
{
    /// <summary>
    /// Represents a single financial transaction such as income, expense,
    /// transfer, or any future financial activity tracked by the system.
    /// Designed for clean architecture and long-term maintainability.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Primary identifier for the transaction entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date the transaction occurred (not the created date).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Short description of the transaction (e.g., "Fuel", "Salary", "Electricity Bill").
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Logical grouping used for budgeting and reporting (e.g., "Transport", "Food", "Bills").
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Positive for income, negative for expense (if using Type instead, this stays raw).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Defines whether the transaction is an expense or income.
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Timestamp when the system created this transaction entry.
        /// Used for audit logs & analytics.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ---------------------------------------------------------------------
        // Derived Properties (Helpful for analytics & UI)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Returns the signed amount based on transaction type.
        /// Income → positive, Expense → negative.
        /// </summary>
        public decimal SignedAmount =>
            Type == TransactionType.Income ? Amount : -Math.Abs(Amount);

        /// <summary>
        /// Indicates whether this transaction occurs in the current month.
        /// Useful for dashboards and quick filtering.
        /// </summary>
        public bool IsCurrentMonth =>
            Date.Month == DateTime.Now.Month && Date.Year == DateTime.Now.Year;

        /// <summary>
        /// Quick check if the transaction is an expense.
        /// </summary>
        public bool IsExpense => Type == TransactionType.Expense;

        /// <summary>
        /// Quick check if the transaction is an income transaction.
        /// </summary>
        public bool IsIncome => Type == TransactionType.Income;

        // ---------------------------------------------------------------------
        // Helper Methods
        // ---------------------------------------------------------------------

        /// <summary>
        /// Provides a user-friendly label for UI (e.g., "Rs 500 - Food").
        /// </summary>
        public string ToDisplayLabel()
        {
            var symbol = IsExpense ? "-" : "+";
            return $"{symbol}{Amount:N2} • {Category}";
        }

        /// <summary>
        /// Ensures a transaction is valid before saving.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty.");

            if (Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (Date == default)
                throw new ArgumentException("Date is not valid.");
        }
    }

    /// <summary>
    /// Defines transaction classification for budgeting & analytics.
    /// </summary>
    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }
}

