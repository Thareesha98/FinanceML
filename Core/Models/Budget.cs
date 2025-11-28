using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceML.Core.Models
{
    /// <summary>
    /// Represents a financial budget with tracking for spending limits,
    /// period type, life cycle state, and progress evaluation.
    /// </summary>
    public class Budget
    {
        /// <summary>
        /// Unique identifier for the budget entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the budget (e.g., "Food", "Transport", "Subscriptions").
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Category this budget belongs to.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Maximum allowed spending for the selected period.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Indicates whether the budget recurs weekly, monthly, or yearly.
        /// </summary>
        [Required]
        public BudgetPeriod Period { get; set; }

        /// <summary>
        /// Start date for the budget cycle.
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// End date for the budget cycle (calculated when created or updated).
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Total spent amount within the period.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal SpentAmount { get; set; }

        /// <summary>
        /// Timestamp for auditing.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether the budget is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ---------------------------------------------------------------------
        // Computed Properties
        // ---------------------------------------------------------------------

        /// <summary>
        /// Returns remaining budget amount (never negative).
        /// </summary>
        public decimal RemainingAmount => Math.Max(0, Amount - SpentAmount);

        /// <summary>
        /// Percentage of budget used (0 to 100+).
        /// </summary>
        public double PercentageUsed =>
            Amount <= 0 ? 0 : Math.Round((double)(SpentAmount / Amount) * 100, 2);

        /// <summary>
        /// Checks if spending exceeded the budget.
        /// </summary>
        public bool IsOverBudget => SpentAmount > Amount;

        /// <summary>
        /// Indicates how many days remain in this budget cycle.
        /// </summary>
        public int DaysLeft =>
            EndDate.Date <= DateTime.Today ? 0 : (EndDate.Date - DateTime.Today).Days;

        // ---------------------------------------------------------------------
        // Helper Methods
        // ---------------------------------------------------------------------

        /// <summary>
        /// Calculates the next budget cycle's end date based on the Period.
        /// </summary>
        public void RecalculateEndDate()
        {
            EndDate = Period switch
            {
                BudgetPeriod.Weekly => StartDate.AddDays(7),
                BudgetPeriod.Monthly => StartDate.AddMonths(1),
                BudgetPeriod.Yearly => StartDate.AddYears(1),
                _ => StartDate
            };
        }

        /// <summary>
        /// Marks the budget as inactive and resets spending.
        /// </summary>
        public void CloseBudget()
        {
            SpentAmount = 0;
            IsActive = false;
        }

        /// <summary>
        /// Determines if the budget has expired.
        /// </summary>
        public bool IsExpired() =>
            DateTime.Now.Date > EndDate.Date;

        /// <summary>
        /// Resets the budget for a new billing cycle.
        /// </summary>
        public void ResetCycle()
        {
            SpentAmount = 0;
            StartDate = DateTime.Now.Date;
            RecalculateEndDate();
        }
    }

    /// <summary>
    /// Defines the available frequency periods for budgets.
    /// </summary>
    public enum BudgetPeriod
    {
        Weekly,
        Monthly,
        Yearly
    }
}

