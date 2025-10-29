using System;

namespace FinanceML.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum TransactionType
    {
        Income,
        Expense
    }
}
