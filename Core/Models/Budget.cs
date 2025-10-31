using System;

namespace FinanceML.Core.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BudgetPeriod Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        public decimal RemainingAmount => Amount - SpentAmount;
        public double PercentageUsed => Amount > 0 ? (double)(SpentAmount / Amount) * 100 : 0;
        public bool IsOverBudget => SpentAmount > Amount;
    }

    public enum BudgetPeriod
    {
        Weekly,
        Monthly,
        Yearly
    }
}
