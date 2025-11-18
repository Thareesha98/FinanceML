using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML.AI
{
    /// <summary>
    /// Provides AI-driven financial insights, predictions, and recommendations.
    /// Rewritten for dependency injection, async support, testability, and SRP compliance.
    /// </summary>
    public class AIInsightsService : IAIInsightsService
    {
        private readonly IDataService _dataService;

        public AIInsightsService(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        // ================================================================
        // 1. Spending Insights (Monthly Comparison, Category Trends, Patterns)
        // ================================================================
        public async Task<List<string>> GetSpendingInsightsAsync()
        {
            var insights = new List<string>();
            var transactions = await _dataService.GetAllTransactionsAsync();

            if (!transactions.Any())
            {
                return new() { "💡 Add your first transaction to start getting insights!" };
            }

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;
            var lastMonth = thisMonth == 1 ? 12 : thisMonth - 1;
            var lastMonthYear = thisMonth == 1 ? thisYear - 1 : thisYear;

            var thisMonthTx = FilterByMonth(transactions, thisMonth, thisYear).ToList();
            var lastMonthTx = FilterByMonth(transactions, lastMonth, lastMonthYear).ToList();

            GenerateMonthlySpendingChangeInsight(thisMonthTx, lastMonthTx, insights);
            GenerateCategoryInsights(thisMonthTx, insights);
            GenerateIncomeVsExpenseInsights(thisMonthTx, insights);
            GenerateFrequencyInsights(thisMonthTx, insights);

            if (!insights.Any())
            {
                insights.Add("📈 Keep tracking your expenses to get more personalized insights!");
            }

            return insights;
        }

        // ================================================================
        // 2. Budget Recommendations
        // ================================================================
        public async Task<List<string>> GetBudgetRecommendationsAsync()
        {
            var recommendations = new List<string>();
            var transactions = await _dataService.GetAllTransactionsAsync();
            var budgets = await _dataService.GetAllBudgetsAsync();

            if (!transactions.Any())
                return new() { "Add transactions to receive budget recommendations." };

            var recentSpending = transactions
                .Where(t => t.Amount < 0 && t.Date >= DateTime.Now.AddMonths(-3))
                .ToList();

            var categoryAverages = recentSpending
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    AvgMonthly = g.Sum(t => Math.Abs(t.Amount)) / 3
                })
                .OrderByDescending(x => x.AvgMonthly)
                .ToList();

            foreach (var category in categoryAverages.Take(5))
            {
                var existing = budgets.FirstOrDefault(b => b.Category == category.Category);
                var suggested = Math.Ceiling(category.AvgMonthly * 1.1m);

                if (existing == null)
                {
                    recommendations.Add($"💡 Create a budget for '{category.Category}': Rs {suggested:N0}/month");
                }
                else if (existing.Amount < category.AvgMonthly)
                {
                    recommendations.Add($"⚠️ Increase '{category.Category}' budget to Rs {suggested:N0}/month");
                }
                else if (existing.Amount > category.AvgMonthly * 1.5m)
                {
                    recommendations.Add($"✅ You can reduce your '{category.Category}' budget");
                }
            }

            if (!recommendations.Any())
            {
                recommendations.Add("✔ Your budgets are well-balanced!");
            }

            return recommendations;
        }

        // ================================================================
        // 3. Savings Goals
        // ================================================================
        public async Task<List<string>> GetSavingsGoalsAsync()
        {
            var goals = new List<string>();
            var transactions = await _dataService.GetAllTransactionsAsync();

            if (!transactions.Any())
                return new() { "Add transactions to generate savings goals." };

            var monthTx = FilterByMonth(transactions, DateTime.Now.Month, DateTime.Now.Year).ToList();
            var income = monthTx.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var expenses = monthTx.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0)
                return new() { "Not enough income data to calculate savings goals." };

            var savings = income - expenses;
            var savingsRate = (savings / income) * 100;

            goals.Add($"🚨 Emergency Fund Target: Rs {(expenses * 6):N0}");

            if (savingsRate < 10)
            {
                goals.Add("🎯 Improve savings to 10% of income.");
                goals.Add($"Goal: Rs {(income * 0.1m):N0} per month.");
            }
            else if (savingsRate < 20)
            {
                goals.Add("🎯 Try reaching 20% savings rate.");
                goals.Add($"Goal: Rs {(income * 0.2m):N0} per month.");
            }
            else
            {
                goals.Add("🌟 Excellent savings rate! Time to start investing.");
            }

            goals.Add($"🏠 Home Savings Goal (5y): Rs {(income * 60):N0}");
            goals.Add($"🚗 Vehicle Fund Goal (1y): Rs {(income * 12):N0}");

            return goals;
        }

        // ================================================================
        // 4. Financial Health Score
        // ================================================================
        public async Task<string> GetFinancialHealthScoreAsync()
        {
            var transactions = await _dataService.GetAllTransactionsAsync();

            if (!transactions.Any())
                return "Add transactions to generate a financial health score.";

            int score = 0;

            var last3Months = DateTime.Now.AddMonths(-3);
            var recent = transactions.Where(t => t.Date >= last3Months).ToList();

            if (recent.Any())
            {
                score += ScoreSavingsRate(recent);
                score += ScoreExpenseDiversity(recent);
                score += ScoreConsistency(recent);

                var budgets = await _dataService.GetAllBudgetsAsync();
                if (budgets.Any())
                    score += 20;
            }

            var level = score switch
            {
                >= 80 => "Excellent 🌟",
                >= 60 => "Good 👍",
                >= 40 => "Fair ⚖️",
                >= 20 => "Needs Improvement 📈",
                _ => "Getting Started 🌱"
            };

            return $"{score}/100 - {level}";
        }

        // ================================================================
        // 5. Spending Predictions
        // ================================================================
        public async Task<List<string>> GetSpendingPredictionsAsync()
        {
            var predictions = new List<string>();
            var transactions = await _dataService.GetAllTransactionsAsync();

            if (transactions.Count < 10)
            {
                return new() { "Add more transaction history for accurate predictions." };
            }

            var last3Months = transactions
                .Where(t => t.Date >= DateTime.Now.AddMonths(-3))
                .ToList();

            var monthlyTrend = last3Months
                .Where(t => t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(t => Math.Abs(t.Amount))
                })
                .OrderBy(x => x.Month)
                .ToList();

            if (monthlyTrend.Count >= 2)
            {
                var avg = monthlyTrend.Average(x => x.Amount);
                var last = monthlyTrend.Last().Amount;

                predictions.Add($"📊 Monthly spending is {(last > avg ? "increasing" : "decreasing")}.");
                predictions.Add($"📅 Predicted next month: Rs {last:N0}");

                var topCategories = last3Months
                    .Where(t => t.Amount < 0)
                    .GroupBy(t => t.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Avg = g.Sum(t => Math.Abs(t.Amount)) / monthlyTrend.Count
                    })
                    .OrderByDescending(x => x.Avg)
                    .Take(3);

                foreach (var c in topCategories)
                {
                    predictions.Add($"🏷 {c.Category}: expected Rs {c.Avg:N0}/month");
                }
            }

            // Seasonal Pattern
            int month = DateTime.Now.Month;
            var seasonal = transactions
                .Where(t => t.Date.Month == month && t.Amount < 0)
                .GroupBy(t => t.Date.Year)
                .Select(g => g.Sum(t => Math.Abs(t.Amount)))
                .ToList();

            if (seasonal.Count > 1)
            {
                predictions.Add(
                    $"📅 Seasonal average for {DateTime.Now:MMMM}: Rs {seasonal.Average():N0}"
                );
            }

            return predictions;
        }

        // ================================================================
        // HELPER LOGIC
        // ================================================================

        private IEnumerable<Transaction> FilterByMonth(IEnumerable<Transaction> tx, int month, int year)
        {
            return tx.Where(t => t.Date.Month == month && t.Date.Year == year);
        }

        private void GenerateMonthlySpendingChangeInsight(List<Transaction> thisMonth, List<Transaction> lastMonth, List<string> insights)
        {
            decimal thisExpenses = thisMonth.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            decimal lastExpenses = lastMonth.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (lastExpenses <= 0)
                return;

            var percent = ((thisExpenses - lastExpenses) / lastExpenses) * 100;

            if (percent > 10)
                insights.Add($"⚠️ Your spending increased by {percent:F1}% this month.");
            else if (percent < -10)
                insights.Add($"✅ You reduced your spending by {Math.Abs(percent):F1}% this month.");
        }

        private void GenerateCategoryInsights(List<Transaction> thisMonthTx, List<string> insights)
        {
            var grouped = thisMonthTx
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => Math.Abs(t.Amount)) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            if (!grouped.Any())
                return;

            var top = grouped.First();
            insights.Add($"📊 Biggest spending category: '{top.Category}' - Rs {top.Amount:N2}");

            if (grouped.Count > 1)
            {
                var second = grouped[1];
                insights.Add($"💡 Try optimizing '{top.Category}' and '{second.Category}' to save more.");
            }
        }

        private void GenerateIncomeVsExpenseInsights(List<Transaction> thisMonthTx, List<string> insights)
        {
            var income = thisMonthTx.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var expenses = thisMonthTx.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0 || expenses <= 0)
                return;

            var rate = ((income - expenses) / income) * 100;

            if (rate > 20)
                insights.Add($"🎯 Excellent! Saving {rate:F1}% this month.");
            else if (rate > 0)
                insights.Add($"💡 Saving {rate:F1}% this month. Aim for 20%.");
            else
                insights.Add($"⚠️ You're spending more than you earn this month.");
        }

        private void GenerateFrequencyInsights(List<Transaction> thisMonthTx, List<string> insights)
        {
            var frequent = thisMonthTx
                .GroupBy(t => t.Description.ToLower())
                .Where(g => g.Count() > 3)
                .Select(g => new
                {
                    Description = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(t => Math.Abs(t.Amount))
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (frequent != null)
            {
                insights.Add($"🔄 You spent Rs {frequent.Amount:N2} across {frequent.Count} transactions for '{frequent.Description}'.");
            }
        }

        private int ScoreSavingsRate(List<Transaction> recent)
        {
            var income = recent.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var expenses = recent.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0)
                return 0;

            var rate = ((income - expenses) / income) * 100;

            return rate switch
            {
                >= 20 => 40,
                >= 10 => 25,
                >= 0 => 10,
                _ => 0
            };
        }

        private int ScoreExpenseDiversity(List<Transaction> recent)
        {
            int count = recent.Where(t => t.Amount < 0).Select(t => t.Category).Distinct().Count();

            return count switch
            {
                >= 5 => 20,
                >= 3 => 15,
                >= 1 => 5,
                _ => 0
            };
        }

        private int ScoreConsistency(List<Transaction> recent)
        {
            int months = recent.GroupBy(t => new { t.Date.Year, t.Date.Month }).Count();

            return months switch
            {
                >= 3 => 20,
                >= 2 => 10,
                _ => 0
            };
        }
    }
}

