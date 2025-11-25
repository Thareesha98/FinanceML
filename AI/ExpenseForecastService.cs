using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML.AI
{
    /// <summary>
    /// Provides expense forecasting using modular engines such as trend,
    /// category analysis, insight generation, and confidence scoring.
    /// </summary>
    public class ExpenseForecastService : IExpenseForecastService
    {
        private readonly IDataService _dataService;
        private readonly ITrendEngine _trendEngine;
        private readonly ICategoryEngine _categoryEngine;
        private readonly IConfidenceEngine _confidenceEngine;
        private readonly IInsightEngine _insightEngine;

        public ExpenseForecastService(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

            // Engines (can be DI injected)
            _trendEngine = new TrendEngine();
            _categoryEngine = new CategoryEngine();
            _confidenceEngine = new ConfidenceEngine();
            _insightEngine = new InsightEngine();
        }

        // ============================================================
        // DATA MODELS
        // ============================================================
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

        // ============================================================
        // MAIN FORECAST ENTRY POINT
        // ============================================================
        public async Task<ForecastResult> GenerateExpenseForecastAsync(int monthsAhead = 6)
        {
            var transactions = await _dataService.GetAllTransactionsAsync();
            var result = new ForecastResult();

            if (transactions.Count < 10)
            {
                result.Insights.Add("Not enough transaction history to generate forecasts.");
                return result;
            }

            result.MonthlyForecasts   = _trendEngine.GenerateMonthlyForecasts(transactions, monthsAhead);
            result.CategoryForecasts  = _categoryEngine.GenerateCategoryForecasts(transactions);

            result.OverallConfidence  = _confidenceEngine.CalculateOverallConfidence(transactions);
            result.TrendDirection     = _trendEngine.CalculateTrendDirection(transactions);

            result.Insights           = _insightEngine.BuildInsights(transactions, result);

            return result;
        }

        // ============================================================
        // HISTORICAL CATEGORY TREND
        // ============================================================
        public async Task<List<ForecastData>> GetCategoryTrendsAsync(string category, int monthsBack = 6)
        {
            var tx = await _dataService.GetAllTransactionsAsync();
            return _categoryEngine.GetCategoryTrendHistory(tx, category, monthsBack);
        }
    }

    // ======================================================================
    // ENGINE 1 — Trend-Based Forecast
    // ======================================================================
    public interface ITrendEngine
    {
        List<ExpenseForecastService.ForecastData> GenerateMonthlyForecasts(List<Transaction> tx, int monthsAhead);
        string CalculateTrendDirection(List<Transaction> tx);
    }

    public class TrendEngine : ITrendEngine
    {
        private const int MinimumHistoryMonths = 3;
        private const int SeasonalAnalysisMonths = 12;

        public List<ExpenseForecastService.ForecastData> GenerateMonthlyForecasts(List<Transaction> tx, int monthsAhead)
        {
            var forecastResults = new List<ExpenseForecastService.ForecastData>();

            var twelveMonthsAgo = DateTime.Now.AddMonths(-SeasonalAnalysisMonths);

            var groupedHistory = tx
                .Where(t => t.Amount < 0 && t.Date >= twelveMonthsAgo)
                .GroupBy(g => new { g.Date.Year, g.Date.Month })
                .Select(g => (
                    Period: new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount: g.Sum(x => Math.Abs(x.Amount))
                ))
                .OrderBy(x => x.Period)
                .ToList();

            if (groupedHistory.Count < MinimumHistoryMonths)
                return forecastResults;

            var basicTrend = LinearTrend(groupedHistory.Select(x => x.Amount).ToList());
            var seasonal = SeasonalFactors(groupedHistory);
            var recentAverage = groupedHistory.TakeLast(3).Average(x => x.Amount);
            var variance = StandardDeviation(groupedHistory.Select(x => x.Amount).ToList());

            for (int i = 1; i <= monthsAhead; i++)
            {
                var nextMonth = DateTime.Now.AddMonths(i);
                var seasonalFactor = seasonal.GetValueOrDefault(nextMonth.Month, 1m);

                var predictedAmount = (recentAverage + (basicTrend * i)) * seasonalFactor;
                var confidence = Math.Max(0.3m, Math.Min(0.95m, 1 - (variance / recentAverage)));

                forecastResults.Add(new ExpenseForecastService.ForecastData
                {
                    Month            = nextMonth,
                    PredictedAmount  = Math.Max(0, predictedAmount),
                    ConfidenceScore  = confidence,
                    Category         = "Total Expenses"
                });
            }

            return forecastResults;
        }

        public string CalculateTrendDirection(List<Transaction> tx)
        {
            var last3Months = DateTime.Now.AddMonths(-3);
            var last6Months = DateTime.Now.AddMonths(-6);

            var recentTotal = tx.Where(t => t.Amount < 0 && t.Date >= last3Months)
                                .Sum(t => Math.Abs(t.Amount));

            var previousTotal = tx.Where(t => t.Amount < 0 && t.Date >= last6Months && t.Date < last3Months)
                                  .Sum(t => Math.Abs(t.Amount));

            if (previousTotal == 0) return "Stable";

            var difference = ((recentTotal - previousTotal) / previousTotal) * 100;

            return difference switch
            {
                > 10  => "Increasing",
                < -10 => "Decreasing",
                _     => "Stable"
            };
        }

        // Utility: Linear Trend
        private decimal LinearTrend(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            int n = values.Count;
            var sumX  = Enumerable.Range(1, n).Sum();
            var sumY  = values.Sum();
            var sumXY = values.Select((val, i) => (i + 1) * val).Sum();
            var sumX2 = Enumerable.Range(1, n).Select(x => x * x).Sum();

            return (n * sumXY - sumX * sumY) /
                   (n * sumX2 - sumX * sumX);
        }

        // Utility: Seasonal factors per month (1–12)
        private Dictionary<int, decimal> SeasonalFactors(List<(DateTime Month, decimal Amount)> data)
        {
            var seasonal = new Dictionary<int, decimal>();
            if (data.Count < SeasonalAnalysisMonths) return seasonal;

            var globalAverage = data.Average(x => x.Amount);

            for (int m = 1; m <= 12; m++)
            {
                var monthData = data.Where(x => x.Month.Month == m).ToList();

                seasonal[m] = monthData.Any()
                    ? monthData.Average(x => x.Amount) / globalAverage
                    : 1m;
            }

            return seasonal;
        }

        // Utility: Standard deviation
        private decimal StandardDeviation(List<decimal> values)
        {
            if (values.Count < 2) return 0;
            
            var mean = values.Average();
            var variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;

            return (decimal)Math.Sqrt((double)variance);
        }
    }

    // ======================================================================
    // ENGINE 2 — Category-Based Forecast
    // ======================================================================
    public interface ICategoryEngine
    {
        List<ExpenseForecastService.ForecastData> GenerateCategoryForecasts(List<Transaction> tx);
        List<ExpenseForecastService.ForecastData> GetCategoryTrendHistory(List<Transaction> tx, string category, int monthsBack);
    }

    public class CategoryEngine : ICategoryEngine
    {
        private const int MinimumSamples = 3;

        public List<ExpenseForecastService.ForecastData> GenerateCategoryForecasts(List<Transaction> tx)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var relevant = tx.Where(t => t.Amount < 0 && t.Date >= sixMonthsAgo);

            var categoryGroups = relevant
                .GroupBy(g => g.Category)
                .Where(g => g.Count() >= MinimumSamples);

            var output = new List<ExpenseForecastService.ForecastData>();

            foreach (var group in categoryGroups)
            {
                var monthlyTotals = group
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => Math.Abs(g.Sum(x => x.Amount)))
                    .ToList();

                if (monthlyTotals.Count < 2) continue;

                var avg     = monthlyTotals.Average();
                var trend   = LinearTrend(monthlyTotals);
                var variance = StandardDeviation(monthlyTotals);

                var nextPrediction = avg + trend;
                var confidence = Math.Max(0.2m, Math.Min(0.9m, 1 - (variance / avg)));

                output.Add(new ExpenseForecastService.ForecastData
                {
                    Month = DateTime.Now.AddMonths(1),
                    PredictedAmount = Math.Max(0, nextPrediction),
                    ConfidenceScore = confidence,
                    Category = group.Key
                });
            }

            return output.OrderByDescending(o => o.PredictedAmount).ToList();
        }

        public List<ExpenseForecastService.ForecastData> GetCategoryTrendHistory(
            List<Transaction> tx,
            string category,
            int monthsBack)
        {
            var start = DateTime.Now.AddMonths(-monthsBack);

            return tx.Where(t => t.Amount < 0 && t.Category == category && t.Date >= start)
                     .GroupBy(g => new { g.Date.Year, g.Date.Month })
                     .Select(g => new ExpenseForecastService.ForecastData
                     {
                         Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                         PredictedAmount = Math.Abs(g.Sum(x => x.Amount)),
                         Category = category,
                         ConfidenceScore = 1m
                     })
                     .OrderBy(d => d.Month)
                     .ToList();
        }

        private decimal LinearTrend(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            int n = values.Count;
            var sumX  = Enumerable.Range(1, n).Sum();
            var sumY  = values.Sum();
            var sumXY = values.Select((val, i) => (i + 1) * val).Sum();
            var sumX2 = Enumerable.Range(1, n).Select(x => x * x).Sum();

            return (n * sumXY - sumX * sumY) /
                   (n * sumX2 - sumX * sumX);
        }

        private decimal StandardDeviation(List<decimal> values)
        {
            if (values.Count < 2) return 0;

            var mean = values.Average();
            var variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;

            return (decimal)Math.Sqrt((double)variance);
        }
    }

    // ======================================================================
    // ENGINE 3 — Confidence Scoring Engine
    // ======================================================================
    public interface IConfidenceEngine
    {
        decimal CalculateOverallConfidence(List<Transaction> tx);
    }

    public class ConfidenceEngine : IConfidenceEngine
    {
        public decimal CalculateOverallConfidence(List<Transaction> tx)
        {
            decimal confidence = 0.5m;     // Base confidence score
            int total = tx.Count;

            if (total >= 100)      confidence += 0.3m;
            else if (total >= 50)  confidence += 0.2m;
            else if (total >= 20)  confidence += 0.1m;

            var recent = tx.Count(t => t.Date >= DateTime.Now.AddMonths(-3));
            if (recent > 20) confidence += 0.1m;

            return Math.Min(0.95m, confidence);
        }
    }

    // ======================================================================
    // ENGINE 4 — Insight Explanation Engine
    // ======================================================================
    public interface IInsightEngine
    {
        List<string> BuildInsights(List<Transaction> tx, ExpenseForecastService.ForecastResult result);
    }

    public class InsightEngine : IInsightEngine
    {
        public List<string> BuildInsights(List<Transaction> tx, ExpenseForecastService.ForecastResult result)
        {
            var messages = new List<string>
            {
                $"Spending Trend: {result.TrendDirection}",
                $"Forecast Confidence: {result.OverallConfidence:P0}"
            };

            var upcoming = result.MonthlyForecasts.FirstOrDefault();
            if (upcoming != null)
            {
                messages.Add($"Next month predicted spending: Rs {upcoming.PredictedAmount:N0}");
            }

            var topCategory = result.CategoryForecasts.FirstOrDefault();
            if (topCategory != null)
            {
                messages.Add(
                    $"Highest category: {topCategory.Category} " +
                    $"(Rs {topCategory.PredictedAmount:N0})"
                );
            }

            messages.Add(SeasonalMessage(DateTime.Now.Month));

            return messages;
        }

        private string SeasonalMessage(int month) => month switch
        {
            12 or 1 or 2  => "Winter: Holiday and utility spending tends to increase.",
            3 or 4 or 5   => "Spring: Good period for expense adjustments.",
            6 or 7 or 8   => "Summer: Travel and entertainment expenses may increase.",
            9 or 10 or 11 => "Festival season: Gift and shopping expenses typically rise.",
            _             => "Seasonal spending analysis can improve financial planning."
        };
    }
}

