using System;
using System.Collections.Generic;
using System.Linq;
using FinanceML.Core.Services;
using FinanceML.Core.Models;

namespace FinanceML.AI
{
    public class ExpenseForecastService
    {
        private static ExpenseForecastService? _instance;
        private readonly DataService _dataService;

        public static ExpenseForecastService Instance => _instance ??= new ExpenseForecastService();

        private ExpenseForecastService()
        {
            _dataService = DataService.Instance;
        }

        public class ForecastData
        {
            public DateTime Month { get; set; }
            public decimal PredictedAmount { get; set; }
            public decimal ConfidenceScore { get; set; }
            public string Category { get; set; } = string.Empty;
        }

        public class ForecastResult
        {
            public List<ForecastData> MonthlyForecasts { get; set; } = new();
            public List<ForecastData> CategoryForecasts { get; set; } = new();
            public decimal OverallConfidence { get; set; }
            public string TrendDirection { get; set; } = string.Empty;
            public List<string> Insights { get; set; } = new();
        }

        public ForecastResult GenerateExpenseForecast(int monthsAhead = 6)
        {
            var transactions = _dataService.GetAllTransactions();
            var result = new ForecastResult();

            if (transactions.Count < 10)
            {
                result.Insights.Add("Need more transaction history for accurate forecasting");
                return result;
            }

            // Generate monthly forecasts
            result.MonthlyForecasts = GenerateMonthlyForecasts(transactions, monthsAhead);
            
            // Generate category forecasts
            result.CategoryForecasts = GenerateCategoryForecasts(transactions, monthsAhead);
            
            // Calculate overall confidence
            result.OverallConfidence = CalculateOverallConfidence(transactions);
            
            // Determine trend direction
            result.TrendDirection = DetermineTrendDirection(transactions);
            
            // Generate insights
            result.Insights = GenerateForecastInsights(transactions, result);

            return result;
        }

        private List<ForecastData> GenerateMonthlyForecasts(List<Transaction> transactions, int monthsAhead)
        {
            var forecasts = new List<ForecastData>();
            
            // Get historical monthly data (last 12 months)
            var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
            var monthlyData = transactions
                .Where(t => t.Date >= twelveMonthsAgo && t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => (
                    Month: new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount: g.Sum(t => Math.Abs(t.Amount))
                ))
                .OrderBy(x => x.Month)
                .ToList();

            if (monthlyData.Count < 3)
            {
                // Not enough data for meaningful forecasting
                return forecasts;
            }

            // Simple linear regression for trend
            var trend = CalculateLinearTrend(monthlyData.Select(x => x.Amount).ToList());
            var seasonalFactors = CalculateSeasonalFactors(monthlyData);
            var baseAmount = monthlyData.TakeLast(3).Average(x => x.Amount);

            for (int i = 1; i <= monthsAhead; i++)
            {
                var forecastMonth = DateTime.Now.AddMonths(i);
                var seasonalFactor = seasonalFactors.GetValueOrDefault(forecastMonth.Month, 1.0m);
                
                var predictedAmount = baseAmount + (trend * i);
                predictedAmount *= seasonalFactor;

                // Add some randomness based on historical variance
                var variance = CalculateVariance(monthlyData.Select(x => x.Amount).ToList());
                var confidence = Math.Max(0.3m, Math.Min(0.95m, 1.0m - (variance / baseAmount)));

                forecasts.Add(new ForecastData
                {
                    Month = forecastMonth,
                    PredictedAmount = Math.Max(0, predictedAmount),
                    ConfidenceScore = confidence,
                    Category = "Total Expenses"
                });
            }

            return forecasts;
        }

        private List<ForecastData> GenerateCategoryForecasts(List<Transaction> transactions, int monthsAhead)
        {
            var forecasts = new List<ForecastData>();
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            
            var categoryData = transactions
                .Where(t => t.Date >= sixMonthsAgo && t.Amount < 0)
                .GroupBy(t => t.Category)
                .Where(g => g.Count() >= 3) // Only categories with sufficient data
                .ToList();

            foreach (var categoryGroup in categoryData)
            {
                var monthlyAmounts = categoryGroup
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => g.Sum(t => Math.Abs(t.Amount)))
                    .ToList();

                if (monthlyAmounts.Count >= 2)
                {
                    var avgAmount = monthlyAmounts.Average();
                    var trend = CalculateLinearTrend(monthlyAmounts);
                    var variance = CalculateVariance(monthlyAmounts);
                    var confidence = Math.Max(0.2m, Math.Min(0.9m, 1.0m - (variance / avgAmount)));

                    var nextMonthPrediction = avgAmount + trend;
                    
                    forecasts.Add(new ForecastData
                    {
                        Month = DateTime.Now.AddMonths(1),
                        PredictedAmount = Math.Max(0, nextMonthPrediction),
                        ConfidenceScore = confidence,
                        Category = categoryGroup.Key
                    });
                }
            }

            return forecasts.OrderByDescending(f => f.PredictedAmount).ToList();
        }

        private decimal CalculateLinearTrend(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            var n = values.Count;
            var sumX = n * (n + 1) / 2; // Sum of 1, 2, 3, ..., n
            var sumY = values.Sum();
            var sumXY = values.Select((y, i) => (i + 1) * y).Sum();
            var sumX2 = n * (n + 1) * (2 * n + 1) / 6; // Sum of squares

            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }

        private Dictionary<int, decimal> CalculateSeasonalFactors(List<(DateTime Month, decimal Amount)> monthlyData)
        {
            var seasonalFactors = new Dictionary<int, decimal>();
            
            if (monthlyData.Count < 12) return seasonalFactors;

            var overallAverage = monthlyData.Average(x => x.Amount);
            
            for (int month = 1; month <= 12; month++)
            {
                var monthData = monthlyData.Where(x => x.Month.Month == month).ToList();
                if (monthData.Any())
                {
                    var monthAverage = monthData.Average(x => x.Amount);
                    seasonalFactors[month] = (decimal)(monthAverage / overallAverage);
                }
                else
                {
                    seasonalFactors[month] = 1.0m;
                }
            }

            return seasonalFactors;
        }

        private decimal CalculateVariance(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            var mean = values.Average();
            var variance = values.Sum(x => (x - mean) * (x - mean)) / values.Count;
            return (decimal)Math.Sqrt((double)variance);
        }

        private decimal CalculateOverallConfidence(List<Transaction> transactions)
        {
            var confidence = 0.5m; // Base confidence

            // More data = higher confidence
            var dataPoints = transactions.Count;
            if (dataPoints >= 100) confidence += 0.3m;
            else if (dataPoints >= 50) confidence += 0.2m;
            else if (dataPoints >= 20) confidence += 0.1m;

            // Recent data = higher confidence
            var recentTransactions = transactions.Count(t => t.Date >= DateTime.Now.AddMonths(-3));
            if (recentTransactions >= 20) confidence += 0.1m;

            // Consistent patterns = higher confidence
            var monthlyVariance = CalculateMonthlyVariance(transactions);
            if (monthlyVariance < 0.3m) confidence += 0.1m;

            return Math.Min(0.95m, confidence);
        }

        private decimal CalculateMonthlyVariance(List<Transaction> transactions)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyTotals = transactions
                .Where(t => t.Date >= sixMonthsAgo && t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => g.Sum(t => Math.Abs(t.Amount)))
                .ToList();

            if (monthlyTotals.Count < 2) return 1.0m;

            var mean = monthlyTotals.Average();
            var variance = monthlyTotals.Sum(x => (x - mean) * (x - mean)) / monthlyTotals.Count;
            return variance / (mean * mean); // Coefficient of variation
        }

        private string DetermineTrendDirection(List<Transaction> transactions)
        {
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var recentExpenses = transactions
                .Where(t => t.Date >= threeMonthsAgo && t.Amount < 0)
                .Sum(t => Math.Abs(t.Amount));

            var olderExpenses = transactions
                .Where(t => t.Date >= sixMonthsAgo && t.Date < threeMonthsAgo && t.Amount < 0)
                .Sum(t => Math.Abs(t.Amount));

            if (olderExpenses == 0) return "Stable";

            var changePercent = ((recentExpenses - olderExpenses) / olderExpenses) * 100;

            return changePercent switch
            {
                > 10 => "Increasing",
                < -10 => "Decreasing",
                _ => "Stable"
            };
        }

        private List<string> GenerateForecastInsights(List<Transaction> transactions, ForecastResult result)
        {
            var insights = new List<string>();

            // Trend insights
            insights.Add($"📈 Spending trend: {result.TrendDirection}");
            insights.Add($"🎯 Forecast confidence: {result.OverallConfidence:P0}");

            // Monthly forecast insights
            if (result.MonthlyForecasts.Any())
            {
                var nextMonth = result.MonthlyForecasts.First();
                insights.Add($"💰 Next month predicted expenses: Rs {nextMonth.PredictedAmount:N0}");

                var currentMonthExpenses = transactions
                    .Where(t => t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year && t.Amount < 0)
                    .Sum(t => Math.Abs(t.Amount));

                if (currentMonthExpenses > 0)
                {
                    var change = ((nextMonth.PredictedAmount - currentMonthExpenses) / currentMonthExpenses) * 100;
                    if (Math.Abs(change) > 5)
                    {
                        var direction = change > 0 ? "increase" : "decrease";
                        insights.Add($"📊 Expected {direction} of {Math.Abs(change):F1}% from current month");
                    }
                }
            }

            // Category insights
            if (result.CategoryForecasts.Any())
            {
                var topCategory = result.CategoryForecasts.First();
                insights.Add($"🏷️ Highest predicted category: {topCategory.Category} (Rs {topCategory.PredictedAmount:N0})");
            }

            // Seasonal insights
            var currentMonth = DateTime.Now.Month;
            var seasonalInsight = currentMonth switch
            {
                12 or 1 or 2 => "❄️ Winter months typically show higher utility expenses",
                3 or 4 or 5 => "🌸 Spring season - good time for budget reviews",
                6 or 7 or 8 => "☀️ Summer months may increase travel and entertainment expenses",
                9 or 10 or 11 => "🍂 Festival season - expect higher shopping and gift expenses",
                _ => "📅 Monitor seasonal spending patterns"
            };
            insights.Add(seasonalInsight);

            return insights;
        }

        public List<ForecastData> GetCategoryTrends(string category, int monthsBack = 6)
        {
            var transactions = _dataService.GetAllTransactions();
            var startDate = DateTime.Now.AddMonths(-monthsBack);

            var categoryTrends = transactions
                .Where(t => t.Category == category && t.Date >= startDate && t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new ForecastData
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    PredictedAmount = g.Sum(t => Math.Abs(t.Amount)),
                    Category = category,
                    ConfidenceScore = 1.0m // Historical data has 100% confidence
                })
                .OrderBy(f => f.Month)
                .ToList();

            return categoryTrends;
        }
    }
}
