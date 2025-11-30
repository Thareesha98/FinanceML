using System;

namespace FinanceML.Core.Models
{
    /// <summary>
    /// Represents a financial entry recorded in the system.
    /// Supports income, expenses, transfers, and future financial event types.
    /// Designed with clean architecture, analytics, and microservice expansion in mind.
    /// </summary>
    public class Transaction
    {
        // ---------------------------------------------------------------------
        // Core Fields
        // ---------------------------------------------------------------------

        /// <summary>
        /// Unique identifier for the transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Business date when the transaction occurred.
        /// </summary>
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// Brief title or explanation (e.g., "Groceries", "Salary").
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Domain category used for budgeting and filtering.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Transaction raw amount. Always stored as a positive decimal.
        /// Sign is determined by TransactionType.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Specifies whether this entry represents income, expense, transfer, etc.
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Timestamp captured when this transaction was created in the system.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Tracks last modification (useful for auditing & syncing logic).
        /// </summary>
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional user ID for multi-user support.
        /// </summary>
        public int? UserId { get; set; }

        // ---------------------------------------------------------------------
        // Derived Properties (Analytics + UI Support)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Returns value with appropriate sign:
        /// - Income  → +Amount
        /// - Expense → -Amount
        /// </summary>
        public decimal SignedAmount =>
            Type == TransactionType.Income ? Amount : -Math.Abs(Amount);

        /// <summary>
        /// Indicates whether the transaction belongs to the current month.
        /// Useful for analytics dashboards.
        /// </summary>
        public bool IsThisMonth =>
            OccurredOn.Month == DateTime.Now.Month &&
            OccurredOn.Year == DateTime.Now.Year;

        /// <summary>
        /// Returns a short label for UI lists.
        /// </summary>
        public string DisplayLabel =>
            $"{(IsExpense ? "-" : "+")}{Amount:N2} • {Category}";

        public bool IsExpense => Type == TransactionType.Expense;
        public bool IsIncome  => Type == TransactionType.Income;

        // ---------------------------------------------------------------------
        // Business Validation
        // ---------------------------------------------------------------------

        /// <summary>
        /// Ensures the transaction meets domain requirements before processing.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Title cannot be empty.");

            if (Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (OccurredOn == default)
                throw new ArgumentException("OccurredOn date must be provided.");

            if (!Enum.IsDefined(typeof(TransactionType), Type))
                throw new ArgumentException("Invalid transaction type.");
        }

        // ---------------------------------------------------------------------
        // Behavior / Mutators
        // ---------------------------------------------------------------------

        /// <summary>
        /// Applies modifications safely and updates audit timestamp.
        /// </summary>
        public void Update(string title, decimal amount, string category, DateTime occurredOn)
        {
            Title = title.Trim();
            Amount = Math.Abs(amount);
            Category = category.Trim();
            OccurredOn = occurredOn;

            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as updated (useful when updating relationships).
        /// </summary>
        public void Touch() => UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Classification of financial entries for analytics and budgeting.
    /// </summary>
    public enum TransactionType
    {
        Income = 1,
        Expense = 2,
        Transfer = 3
    }
}

