using System;

namespace FinanceML.Models
{
    public class Transaction
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "Income" or "Expense"

        public Transaction(string description, decimal amount, DateTime date, string type)
        {
            Description = description;
            Amount = amount;
            Date = date;
            Type = type;
        }
    }
}

