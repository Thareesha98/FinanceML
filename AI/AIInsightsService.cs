using System;
using System.Collections.Generic;
using System.Linq;
using FinanceML.Core.Services;
using FinanceML.Core.Models;

namespace FinanceML.AI
{
    public class AIInsightsService
    {
        private static AIInsightsService? _instance;
        private readonly DataService _dataService;

        public static AIInsightsService Instance => _instance ??= new AIInsightsService();

        private AIInsightsService()
        {
            _dataService = DataService.Instance;
        }

        public List<string> GetSpendingInsights()
        {
            var insights = new List<string>();
            var transactions = _dataService.GetAllTransactions();
            
            if (!transactions.Any())
            {
                insights.Add("💡 Start by adding some transactions to get personalized insights!");
                return insights;
            }

            // Analyze spending patterns
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var thisMonthTransactions = transactions.Where(t => 
                t.Date.Month == currentMonth && t.Date.Year == currentYear).ToList();
            
            var lastMonthTransactions = transactions.Where(t => 
                t.Date.Month == (currentMonth == 1 ? 12 : currentMonth - 1) && 
                t.Date.Year == (currentMonth == 1 ? currentYear - 1 : currentYear)).ToList();

            // Monthly spending comparison
            var thisMonthExpenses = thisMonthTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            var lastMonthExpenses = lastMonthTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            
            if (lastMonthExpenses > 0)
            {
                var changePercent = ((thisMonthExpenses - lastMonthExpenses) / lastMonthExpenses) * 100;
                if (changePercent > 10)
                {
                    insights.Add($"⚠️ Your spending increased by {changePercent:F1}% this month. Consider reviewing your budget.");
                }
                else if (changePercent < -10)
                {
                    insights.Add($"✅ Great job! You reduced spending by {Math.Abs(changePercent):F1}% this month.");
                }
            }

            // Category analysis
            var categorySpending = thisMonthTransactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => Math.Abs(t.Amount)) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            if (categorySpending.Any())
            {
                var topCategory = categorySpending.First();
                insights.Add($"📊 Your highest spending category this month is '{topCategory.Category}' at Rs {topCategory.Amount:N2}");
                
                if (categorySpending.Count > 1)
                {
                    var secondCategory = categorySpending[1];
                    insights.Add($"💰 Consider optimizing '{topCategory.Category}' and '{secondCategory.Category}' for better savings");
                }
            }

            // Income vs Expenses
            var totalIncome = thisMonthTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var totalExpenses = thisMonthTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            
            if (totalIncome > 0 && totalExpenses > 0)
            {
                var savingsRate = ((totalIncome - totalExpenses) / totalIncome) * 100;
                if (savingsRate > 20)
                {
                    insights.Add($"🎯 Excellent! You're saving {savingsRate:F1}% of your income this month.");
                }
                else if (savingsRate > 0)
                {
                    insights.Add($"💡 You're saving {savingsRate:F1}% this month. Try to reach 20% for optimal financial health.");
                }
                else
                {
                    insights.Add("⚠️ You're spending more than you earn this month. Consider reducing expenses.");
                }
            }

            // Frequency insights
            var frequentTransactions = thisMonthTransactions
                .GroupBy(t => t.Description.ToLower())
                .Where(g => g.Count() > 3)
                .Select(g => new { Description = g.Key, Count = g.Count(), Amount = g.Sum(t => Math.Abs(t.Amount)) })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (frequentTransactions != null)
            {
                insights.Add($"🔄 You have {frequentTransactions.Count} transactions for '{frequentTransactions.Description}' totaling Rs {frequentTransactions.Amount:N2}");
            }

            if (!insights.Any())
            {
                insights.Add("📈 Keep tracking your expenses to get more personalized insights!");
                insights.Add("💡 Try categorizing your transactions for better analysis.");
            }

            return insights;
        }

        public List<string> GetBudgetRecommendations()
        {
            var recommendations = new List<string>();
            var transactions = _dataService.GetAllTransactions();
            var budgets = _dataService.GetAllBudgets();

            if (!transactions.Any())
            {
                recommendations.Add("Start by adding transactions to get budget recommendations");
                return recommendations;
            }

            // Analyze spending by category over last 3 months
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);
            var recentTransactions = transactions.Where(t => t.Date >= threeMonthsAgo && t.Amount < 0).ToList();

            var categoryAverages = recentTransactions
                .GroupBy(t => t.Category)
                .Select(g => new { 
                    Category = g.Key, 
                    MonthlyAverage = g.Sum(t => Math.Abs(t.Amount)) / 3 
                })
                .OrderByDescending(x => x.MonthlyAverage)
                .ToList();

            foreach (var category in categoryAverages.Take(5))
            {
                var existingBudget = budgets.FirstOrDefault(b => b.Category == category.Category);
                var suggestedAmount = Math.Ceiling(category.MonthlyAverage * 1.1m); // 10% buffer

                if (existingBudget == null)
                {
                    recommendations.Add($"💡 Create a budget for '{category.Category}': Rs {suggestedAmount:N0}/month");
                }
                else if (existingBudget.Amount < category.MonthlyAverage)
                {
                    recommendations.Add($"⚠️ Increase '{category.Category}' budget to Rs {suggestedAmount:N0}/month");
                }
                else if (existingBudget.Amount > category.MonthlyAverage * 1.5m)
                {
                    recommendations.Add($"✅ You can reduce '{category.Category}' budget to Rs {suggestedAmount:N0}/month");
                }
            }

            if (!recommendations.Any())
            {
                recommendations.Add("Your current budgets look well-balanced!");
                recommendations.Add("Continue monitoring your spending patterns.");
            }

            return recommendations;
        }

        public List<string> GetSavingsGoals()
        {
            var goals = new List<string>();
            var transactions = _dataService.GetAllTransactions();

            if (!transactions.Any())
            {
                goals.Add("Add transactions to get personalized savings goals");
                return goals;
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var thisMonthTransactions = transactions.Where(t => 
                t.Date.Month == currentMonth && t.Date.Year == currentYear).ToList();

            var monthlyIncome = thisMonthTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var monthlyExpenses = thisMonthTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (monthlyIncome > 0)
            {
                var currentSavings = monthlyIncome - monthlyExpenses;
                var savingsRate = (currentSavings / monthlyIncome) * 100;

                // Emergency fund goal
                var emergencyFundGoal = monthlyExpenses * 6;
                goals.Add($"🚨 Emergency Fund: Save Rs {emergencyFundGoal:N0} (6 months of expenses)");

                // Short-term savings goals
                if (savingsRate < 10)
                {
                    goals.Add("🎯 Short-term: Aim to save 10% of your income");
                    goals.Add($"💰 Target: Rs {(monthlyIncome * 0.1m):N0} per month");
                }
                else if (savingsRate < 20)
                {
                    goals.Add("🎯 Medium-term: Increase savings to 20% of income");
                    goals.Add($"💰 Target: Rs {(monthlyIncome * 0.2m):N0} per month");
                }
                else
                {
                    goals.Add("🌟 Excellent savings rate! Consider investment opportunities");
                    goals.Add($"📈 Surplus: Rs {currentSavings:N0} available for investments");
                }

                // Long-term goals
                goals.Add($"🏠 Home Down Payment: Rs {(monthlyIncome * 60):N0} (5 years of savings)");
                goals.Add($"🚗 Vehicle Fund: Rs {(monthlyIncome * 12):N0} (1 year of savings)");
            }

            return goals;
        }

        public string GetFinancialHealthScore()
        {
            var transactions = _dataService.GetAllTransactions();
            
            if (!transactions.Any())
            {
                return "Add transactions to calculate your financial health score";
            }

            var score = 0;
            var factors = new List<string>();

            // Recent transactions (last 3 months)
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);
            var recentTransactions = transactions.Where(t => t.Date >= threeMonthsAgo).ToList();

            if (recentTransactions.Any())
            {
                var totalIncome = recentTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
                var totalExpenses = recentTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

                // Savings rate (40 points max)
                if (totalIncome > 0)
                {
                    var savingsRate = ((totalIncome - totalExpenses) / totalIncome) * 100;
                    if (savingsRate >= 20) { score += 40; factors.Add("Excellent savings rate"); }
                    else if (savingsRate >= 10) { score += 25; factors.Add("Good savings rate"); }
                    else if (savingsRate >= 0) { score += 10; factors.Add("Positive cash flow"); }
                    else { factors.Add("Spending exceeds income"); }
                }

                // Expense diversity (20 points max)
                var categories = recentTransactions.Where(t => t.Amount < 0).Select(t => t.Category).Distinct().Count();
                if (categories >= 5) { score += 20; factors.Add("Well-diversified spending"); }
                else if (categories >= 3) { score += 15; factors.Add("Moderate spending diversity"); }
                else if (categories >= 1) { score += 5; factors.Add("Limited spending categories"); }

                // Transaction consistency (20 points max)
                var monthsWithTransactions = recentTransactions.GroupBy(t => new { t.Date.Year, t.Date.Month }).Count();
                if (monthsWithTransactions >= 3) { score += 20; factors.Add("Consistent tracking"); }
                else if (monthsWithTransactions >= 2) { score += 10; factors.Add("Regular tracking"); }

                // Budget adherence (20 points max)
                var budgets = _dataService.GetAllBudgets();
                if (budgets.Any())
                {
                    score += 20;
                    factors.Add("Active budget management");
                }
            }

            var healthLevel = score switch
            {
                >= 80 => "Excellent 🌟",
                >= 60 => "Good 👍",
                >= 40 => "Fair ⚖️",
                >= 20 => "Needs Improvement 📈",
                _ => "Getting Started 🌱"
            };

            return $"{score}/100 - {healthLevel}";
        }

        public List<string> GetSpendingPredictions()
        {
            var predictions = new List<string>();
            var transactions = _dataService.GetAllTransactions();

            if (transactions.Count < 10)
            {
                predictions.Add("Need more transaction history for accurate predictions");
                return predictions;
            }

            // Analyze last 3 months for trends
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);
            var recentTransactions = transactions.Where(t => t.Date >= threeMonthsAgo).ToList();

            // Monthly spending trend
            var monthlySpending = recentTransactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new { 
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(t => Math.Abs(t.Amount))
                })
                .OrderBy(x => x.Month)
                .ToList();

            if (monthlySpending.Count >= 2)
            {
                var avgSpending = monthlySpending.Average(x => x.Amount);
                var lastMonth = monthlySpending.Last().Amount;
                var trend = lastMonth > avgSpending ? "increasing" : "decreasing";
                
                predictions.Add($"📊 Monthly spending trend: {trend}");
                predictions.Add($"💰 Predicted next month: Rs {lastMonth:N0}");
                
                // Category predictions
                var topCategories = recentTransactions
                    .Where(t => t.Amount < 0)
                    .GroupBy(t => t.Category)
                    .Select(g => new { 
                        Category = g.Key, 
                        MonthlyAvg = g.Sum(t => Math.Abs(t.Amount)) / monthlySpending.Count 
                    })
                    .OrderByDescending(x => x.MonthlyAvg)
                    .Take(3)
                    .ToList();

                foreach (var category in topCategories)
                {
                    predictions.Add($"🏷️ {category.Category}: Rs {category.MonthlyAvg:N0}/month expected");
                }
            }

            // Seasonal patterns
            var currentMonth = DateTime.Now.Month;
            var seasonalSpending = transactions
                .Where(t => t.Date.Month == currentMonth && t.Amount < 0)
                .GroupBy(t => t.Date.Year)
                .Select(g => g.Sum(t => Math.Abs(t.Amount)))
                .ToList();

            if (seasonalSpending.Count > 1)
            {
                var avgSeasonalSpending = seasonalSpending.Average();
                predictions.Add($"📅 Historical {DateTime.Now:MMMM} average: Rs {avgSeasonalSpending:N0}");
            }

            return predictions;
        }
    }
}
