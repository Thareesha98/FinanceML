using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML.AI
{
    /// <summary>
    /// AI-driven forecasting engine with support for DI, async operations,
    /// modular engines, seasonal patterns, and high-precision trend analytics.
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

            // Sub-engines (can be DI-injected in advanced users)
            _trendEngine = new TrendEngine();
            _categoryEngine = new CategoryEngine();
            _confidenceEngine = new ConfidenceEngine();
            _insightEngine = new InsightEngine();
        }

        // ============================================================
        // FORECAST RESULT MODELS
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
        // MAIN ENTRY POINT — ASYNC
        // ============================================================
        public async Task<ForecastResult> GenerateExpenseForecastAsync(int monthsAhead = 6)
        {
            var transactions = await _dataService.GetAllTransactionsAsync();
            var result = new ForecastResult();

            if (transactions.Count < 10)
            {
                result.Insights.Add("📉 Not enough transaction history for forecasting.");
                return result;
            }

            // Monthly & Category predictions
            result.MonthlyForecasts = _trendEngine.GenerateMonthlyForecasts(transactions, monthsAhead);
            result.CategoryForecasts = _categoryEngine.GenerateCategoryForecasts(transactions);

            // Confidence + Trend Direction
            result.OverallConfidence = _confidenceEngine.CalculateOverallConfidence(transactions);
            result.TrendDirection = _trendEngine.CalculateTrendDirection(transactions);

            // Insights summary
            result.Insights = _insightEngine.BuildInsights(transactions, result);

            return result;
        }

        // ============================================================
        // CATEGORY TREND HISTORY
        // ============================================================
        public async Task<List<ForecastData>> GetCategoryTrendsAsync(string category, int monthsBack = 6)
        {
            var tx = await _dataService.GetAllTransactionsAsync();
            return _categoryEngine.GetCategoryTrendHistory(tx, category, monthsBack);
        }
    }

    // ======================================================================
    // ENGINE #1: TREND + MONTHLY FORECAST ENGINE
    // ======================================================================
    public interface ITrendEngine
    {
        List<ExpenseForecastService.ForecastData> GenerateMonthlyForecasts(List<Transaction> tx, int monthsAhead);
        string CalculateTrendDirection(List<Transaction> tx);
    }

    public class TrendEngine : ITrendEngine
    {
        public List<ExpenseForecastService.ForecastData> GenerateMonthlyForecasts(List<Transaction> tx, int monthsAhead)
        {
            var output = new List<ExpenseForecastService.ForecastData>();

            var twelveMonthsAgo = DateTime.Now.AddMonths(-12);

            var history = tx
                .Where(t => t.Amount < 0 && t.Date >= twelveMonthsAgo)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => (
                    Date: new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount: g.Sum(x => Math.Abs(x.Amount))
                ))
                .OrderBy(x => x.Date)
                .ToList();

            if (history.Count < 3)
                return output;

            var trend = LinearTrend(history.Select(h => h.Amount).ToList());
            var seasonal = SeasonalFactors(history);
            var baseAmount = history.TakeLast(3).Average(h => h.Amount);
            var variance = Variance(history.Select(h => h.Amount).ToList());

            for (int i = 1; i <= monthsAhead; i++)
            {
                var month = DateTime.Now.AddMonths(i);
                var season = seasonal.GetValueOrDefault(month.Month, 1.0m);

                var predicted = baseAmount + (trend * i);
                predicted *= season;

                var confidence = Math.Max(0.3m, Math.Min(0.95m, 1 - (variance / baseAmount)));

                output.Add(new ExpenseForecastService.ForecastData
                {
                    Month = month,
                    PredictedAmount = Math.Max(0, predicted),
                    ConfidenceScore = confidence,
                    Category = "Total Expenses"
                });
            }

            return output;
        }

        public string CalculateTrendDirection(List<Transaction> tx)
        {
            var threeMonths = DateTime.Now.AddMonths(-3);
            var sixMonths = DateTime.Now.AddMonths(-6);

            var recent = tx.Where(t => t.Amount < 0 && t.Date >= threeMonths)
                           .Sum(t => Math.Abs(t.Amount));

            var older = tx.Where(t => t.Amount < 0 && t.Date >= sixMonths && t.Date < threeMonths)
                          .Sum(t => Math.Abs(t.Amount));

            if (older == 0) return "Stable";

            var pct = ((recent - older) / older) * 100;

            return pct switch
            {
                > 10 => "Increasing",
                < -10 => "Decreasing",
                _ => "Stable"
            };
        }

        // ------------------------ UTILITIES ------------------------

        private decimal LinearTrend(List<decimal> vals)
        {
            if (vals.Count < 2) return 0;
            int n = vals.Count;

            var sumX = n * (n + 1) / 2;
            var sumY = vals.Sum();
            var sumXY = vals.Select((y, i) => (i + 1) * y).Sum();
            var sumX2 = n * (n + 1) * (2 * n + 1) / 6;

            return (n * sumXY - sumX * sumY) /
                   (n * sumX2 - sumX * sumX);
        }

        private Dictionary<int, decimal> SeasonalFactors(List<(DateTime Month, decimal Amount)> data)
        {
            var map = new Dictionary<int, decimal>();
            if (data.Count < 12) return map;

            var avg = data.Average(x => x.Amount);

            for (int m = 1; m <= 12; m++)
            {
                var items = data.Where(d => d.Month.Month == m).ToList();
                if (!items.Any()) { map[m] = 1.0m; continue; }

                map[m] = items.Average(x => x.Amount) / avg;
            }

            return map;
        }

        private decimal Variance(List<decimal> vals)
        {
            if (vals.Count < 2) return 0;
            var mean = vals.Average();
            var var = vals.Sum(v => (v - mean) * (v - mean)) / vals.Count;
            return (decimal)Math.Sqrt((double)var);
        }
    }

    // ======================================================================
    // ENGINE #2: CATEGORY FORECAST ENGINE
    // ======================================================================
    public interface ICategoryEngine
    {
        List<ExpenseForecastService.ForecastData> GenerateCategoryForecasts(List<Transaction> tx);
        List<ExpenseForecastService.ForecastData> GetCategoryTrendHistory(List<Transaction> tx, string category, int monthsBack);
    }

    public class CategoryEngine : ICategoryEngine
    {
        public List<ExpenseForecastService.ForecastData> GenerateCategoryForecasts(List<Transaction> tx)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var groups = tx
                .Where(t => t.Amount < 0 && t.Date >= sixMonthsAgo)
                .GroupBy(t => t.Category)
                .Where(g => g.Count() >= 3)
                .ToList();

            var results = new List<ExpenseForecastService.ForecastData>();

            foreach (var g in groups)
            {
                var monthly = g.GroupBy(t => new { t.Date.Year, t.Date.Month })
                               .Select(h => Math.Abs(h.Sum(x => x.Amount)))
                               .ToList();

                if (monthly.Count < 2) continue;

                var avg = monthly.Average();
                var slope = LinearTrend(monthly);
                var variance = Variance(monthly);

                var prediction = avg + slope;
                var confidence = Math.Max(0.2m, Math.Min(0.9m, 1 - (variance / avg)));

                results.Add(new ExpenseForecastService.ForecastData
                {
                    Month = DateTime.Now.AddMonths(1),
                    PredictedAmount = Math.Max(0, prediction),
                    ConfidenceScore = confidence,
                    Category = g.Key
                });
            }

            return results.OrderByDescending(f => f.PredictedAmount).ToList();
        }

        public List<ExpenseForecastService.ForecastData> GetCategoryTrendHistory(List<Transaction> tx, string category, int monthsBack)
        {
            var start = DateTime.Now.AddMonths(-monthsBack);

            return tx.Where(t => t.Amount < 0 && t.Category == category && t.Date >= start)
                     .GroupBy(t => new { t.Date.Year, t.Date.Month })
                     .Select(g => new ExpenseForecastService.ForecastData
                     {
                         Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                         PredictedAmount = Math.Abs(g.Sum(x => x.Amount)),
                         Category = category,
                         ConfidenceScore = 1.0m
                     })
                     .OrderBy(f => f.Month)
                     .ToList();
        }

        // reuse from trend engine
        private decimal LinearTrend(List<decimal> vals)
        {
            if (vals.Count < 2) return 0;
            int n = vals.Count;

            var sumX = n * (n + 1) / 2;
            var sumY = vals.Sum();
            var sumXY = vals.Select((y, i) => (i + 1) * y).Sum();
            var sumX2 = n * (n + 1) * (2 * n + 1) / 6;

            return (n * sumXY - sumX * sumY) /
                   (n * sumX2 - sumX * sumX);
        }

        private decimal Variance(List<decimal> vals)
        {
            if (vals.Count < 2) return 0;
            var mean = vals.Average();
            var var = vals.Sum(v => (v - mean) * (v - mean)) / vals.Count;
            return (decimal)Math.Sqrt((double)var);
        }
    }

    // ======================================================================
    // ENGINE #3: CONFIDENCE ENGINE
    // ======================================================================
    public interface IConfidenceEngine
    {
        decimal CalculateOverallConfidence(List<Transaction> tx);
    }

    public class ConfidenceEngine : IConfidenceEngine
    {
        public decimal CalculateOverallConfidence(List<Transaction> tx)
        {
            decimal conf = 0.5m; // base
            int count = tx.Count;

            if (count >= 100) conf += 0.3m;
            else if (count >= 50) conf += 0.2m;
            else if (count >= 20) conf += 0.1m;

            var recent = tx.Count(t => t.Date >= DateTime.Now.AddMonths(-3));
            if (recent > 20) conf += 0.1m;

            return Math.Min(0.95m, conf);
        }
    }

    // ======================================================================
    // ENGINE #4: INSIGHT ENGINE
    // ======================================================================
    public interface IInsightEngine
    {
        List<string> BuildInsights(List<Transaction> tx, ExpenseForecastService.ForecastResult result);
    }

    public class InsightEngine : IInsightEngine
    {
        public List<string> BuildInsights(List<Transaction> tx, ExpenseForecastService.ForecastResult result)
        {
            var insights = new List<string>();

            insights.Add($"📈 Spending Trend: {result.TrendDirection}");
            insights.Add($"🎯 Overall Forecast Confidence: {result.OverallConfidence:P0}");

            // Next month
            var next = result.MonthlyForecasts.FirstOrDefault();
            if (next != null)
                insights.Add($"💰 Predicted next month expenses: Rs {next.PredictedAmount:N0}");

            // Category insights
            var top = result.CategoryForecasts.FirstOrDefault();
            if (top != null)
                insights.Add($"🏷 Highest predicted category: {top.Category} (Rs {top.PredictedAmount:N0})");

            // Seasonal insight
            insights.Add(SeasonalMessage(DateTime.Now.Month));

            return insights;
        }

        private string SeasonalMessage(int month) =>
            month switch
            {
                12 or 1 or 2 => "❄ Winter: Higher utility and holiday spending.",
                3 or 4 or 5 => "🌸 Spring: Good time for budget adjustments.",
                6 or 7 or 8 => "☀ Summer: Expect more travel/entertainment costs.",
                9 or 10 or 11 => "🍂 Festival season: Shopping and gifts increase.",
                _ => "📅 Track seasonal patterns for better accuracy."
            };
    }
}

