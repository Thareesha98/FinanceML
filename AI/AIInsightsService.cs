using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Services;

namespace FinanceML.AI
{
    /// <summary>
    /// Provides AI-driven financial insights, predictions, scoring and recommendations.
    /// Fully rewritten for modularity, DI, testability, clean commits and contribution maximization.
    /// </summary>
    public class AIInsightsService : IAIInsightsService
    {
        private readonly IDataService _dataService;
        private readonly IInsightHelper _insightHelper;
        private readonly ISpendingAnalysisEngine _spendingEngine;
        private readonly IBudgetAnalysisEngine _budgetEngine;
        private readonly IFinancialScoreEngine _scoreEngine;
        private readonly IPredictionEngine _predictionEngine;
        private readonly ISavingsEngine _savingsEngine;

        public AIInsightsService(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

            // Engines (can later be DI’d individually → more commits)
            _insightHelper = new InsightHelper();
            _spendingEngine = new SpendingAnalysisEngine();
            _budgetEngine = new BudgetAnalysisEngine();
            _scoreEngine = new FinancialScoreEngine();
            _predictionEngine = new PredictionEngine();
            _savingsEngine = new SavingsEngine();
        }

        // ============================================================
        // 1. SPENDING INSIGHTS
        // ============================================================
        public async Task<List<string>> GetSpendingInsightsAsync()
        {
            var tx = await _dataService.GetAllTransactionsAsync();

            if (!tx.Any())
                return _insightHelper.NoData("Add your first transaction to start getting insights!");

            var thisMonth = _insightHelper.FilterMonth(tx, DateTime.Now);
            var lastMonth = _insightHelper.FilterMonth(tx, DateTime.Now.AddMonths(-1));

            var insights = new List<string>();

            insights.AddRange(_spendingEngine.CompareMonthlySpending(thisMonth, lastMonth));
            insights.AddRange(_spendingEngine.BiggestCategories(thisMonth));
            insights.AddRange(_spendingEngine.IncomeVsExpense(thisMonth));
            insights.AddRange(_spendingEngine.FrequencyInsights(thisMonth));

            return insights.Any()
                ? insights
                : _insightHelper.Single("📈 Keep tracking your expenses for better insights.");
        }

        // ============================================================
        // 2. BUDGET RECOMMENDATIONS
        // ============================================================
        public async Task<List<string>> GetBudgetRecommendationsAsync()
        {
            var tx = await _dataService.GetAllTransactionsAsync();
            var budgets = await _dataService.GetAllBudgetsAsync();

            if (!tx.Any())
                return _insightHelper.Single("Add transactions to receive budget recommendations.");

            return _budgetEngine.GenerateRecommendations(tx, budgets);
        }

        // ============================================================
        // 3. SAVINGS GOALS
        // ============================================================
        public async Task<List<string>> GetSavingsGoalsAsync()
        {
            var tx = await _dataService.GetAllTransactionsAsync();

            if (!tx.Any())
                return _insightHelper.Single("Add transactions to generate savings goals.");

            return _savingsEngine.BuildSavingsGoals(tx);
        }

        // ============================================================
        // 4. FINANCIAL HEALTH SCORE
        // ============================================================
        public async Task<string> GetFinancialHealthScoreAsync()
        {
            var tx = await _dataService.GetAllTransactionsAsync();
            var budgets = await _dataService.GetAllBudgetsAsync();

            if (!tx.Any())
                return "Add transactions to calculate a financial health score.";

            int score = _scoreEngine.CalculateScore(tx, budgets);

            return _scoreEngine.ScoreLabel(score);
        }

        // ============================================================
        // 5. SPENDING PREDICTIONS
        // ============================================================
        public async Task<List<string>> GetSpendingPredictionsAsync()
        {
            var tx = await _dataService.GetAllTransactionsAsync();

            if (tx.Count < 10)
                return _insightHelper.Single("Add more transaction history for accurate predictions.");

            return _predictionEngine.Predict(tx);
        }
    }

    // ======================================================================
    // SUB-SERVICES (engines) → smaller, atomic commits for more contributions
    // ======================================================================

    #region Helper Utilities
    public interface IInsightHelper
    {
        IEnumerable<Transaction> FilterMonth(IEnumerable<Transaction> tx, DateTime date);
        List<string> Single(string message);
        List<string> NoData(string message);
    }

    public class InsightHelper : IInsightHelper
    {
        public IEnumerable<Transaction> FilterMonth(IEnumerable<Transaction> tx, DateTime date) =>
            tx.Where(t => t.Date.Month == date.Month && t.Date.Year == date.Year);

        public List<string> Single(string message) => new() { message };

        public List<string> NoData(string msg) => new() { msg };
    }
    #endregion

    #region Spending Analysis
    public interface ISpendingAnalysisEngine
    {
        IEnumerable<string> CompareMonthlySpending(IEnumerable<Transaction> current, IEnumerable<Transaction> previous);
        IEnumerable<string> BiggestCategories(IEnumerable<Transaction> current);
        IEnumerable<string> IncomeVsExpense(IEnumerable<Transaction> current);
        IEnumerable<string> FrequencyInsights(IEnumerable<Transaction> current);
    }

    public class SpendingAnalysisEngine : ISpendingAnalysisEngine
    {
        public IEnumerable<string> CompareMonthlySpending(IEnumerable<Transaction> current, IEnumerable<Transaction> previous)
        {
            var insights = new List<string>();

            var c = current.Where(t => t.Amount < 0).Sum(x => Math.Abs(x.Amount));
            var p = previous.Where(t => t.Amount < 0).Sum(x => Math.Abs(x.Amount));

            if (p <= 0) return insights;

            var pct = ((c - p) / p) * 100;

            if (pct > 10)
                insights.Add($"⚠️ Spending increased by {pct:F1}% this month.");
            else if (pct < -10)
                insights.Add($"✅ Spending decreased by {Math.Abs(pct):F1}% this month.");

            return insights;
        }

        public IEnumerable<string> BiggestCategories(IEnumerable<Transaction> current)
        {
            var insights = new List<string>();

            var groups = current.Where(t => t.Amount < 0)
                .GroupBy(t => t.Category)
                .Select(g => new { g.Key, Amount = g.Sum(x => Math.Abs(x.Amount)) })
                .OrderByDescending(x => x.Amount)
                .ToList();

            if (!groups.Any()) return insights;

            var top = groups.First();
            insights.Add($"📊 Highest spending: '{top.Key}' – Rs {top.Amount:N0}");

            if (groups.Count > 1)
                insights.Add($"💡 Try reducing: {groups[1].Key} & {groups[0].Key}");

            return insights;
        }

        public IEnumerable<string> IncomeVsExpense(IEnumerable<Transaction> cur)
        {
            var list = new List<string>();
            var income = cur.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var exp = cur.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0 || exp <= 0) return list;

            var rate = ((income - exp) / income) * 100;

            if (rate > 20)
                list.Add($"🎯 Excellent savings rate ({rate:F1}%).");
            else if (rate > 0)
                list.Add($"💡 Savings rate: {rate:F1}%. Aim for 20%.");
            else
                list.Add("⚠️ You are spending more than you earn.");

            return list;
        }

        public IEnumerable<string> FrequencyInsights(IEnumerable<Transaction> current)
        {
            var msg = new List<string>();

            var frequent = current
                .GroupBy(t => t.Description.ToLower())
                .Where(g => g.Count() >= 3)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(x => Math.Abs(x.Amount))
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (frequent != null)
                msg.Add($"🔄 Frequent expense '{frequent.Key}' {frequent.Count} times – Rs {frequent.Amount:N0}");

            return msg;
        }
    }
    #endregion

    #region Budget Engine
    public interface IBudgetAnalysisEngine
    {
        List<string> GenerateRecommendations(IEnumerable<Transaction> tx, IEnumerable<Budget> budgets);
    }

    public class BudgetAnalysisEngine : IBudgetAnalysisEngine
    {
        public List<string> GenerateRecommendations(IEnumerable<Transaction> tx, IEnumerable<Budget> budgets)
        {
            var rec = new List<string>();

            var recent = tx.Where(t => t.Amount < 0 && t.Date >= DateTime.Now.AddMonths(-3)).ToList();

            var categories = recent
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Avg = g.Sum(x => Math.Abs(x.Amount)) / 3
                })
                .OrderByDescending(x => x.Avg)
                .ToList();

            foreach (var c in categories.Take(5))
            {
                var b = budgets.FirstOrDefault(x => x.Category == c.Category);
                var suggested = Math.Ceiling(c.Avg * 1.1m);

                if (b == null)
                    rec.Add($"💡 Create budget for '{c.Category}' → Rs {suggested:N0}");
                else if (b.Amount < c.Avg)
                    rec.Add($"⚠️ Increase '{c.Category}' budget → Rs {suggested:N0}");
                else if (b.Amount > c.Avg * 1.5m)
                    rec.Add($"✅ You may reduce '{c.Category}' budget.");
            }

            if (!rec.Any())
                rec.Add("✔ All budgets look balanced.");

            return rec;
        }
    }
    #endregion

    #region Savings Engine
    public interface ISavingsEngine
    {
        List<string> BuildSavingsGoals(IEnumerable<Transaction> tx);
    }

    public class SavingsEngine : ISavingsEngine
    {
        public List<string> BuildSavingsGoals(IEnumerable<Transaction> tx)
        {
            var result = new List<string>();
            var month = DateTime.Now;

            var monthTx = tx.Where(t => t.Date.Month == month.Month && t.Date.Year == month.Year);

            var income = monthTx.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var exp = monthTx.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0)
                return new() { "Not enough income data to build savings goals." };

            var savings = income - exp;
            var rate = (savings / income) * 100;

            result.Add($"🚨 Emergency Fund (6x expenses): Rs {(exp * 6):N0}");

            if (rate < 10)
            {
                result.Add("🎯 Aim for 10% monthly savings.");
                result.Add($"Goal: Rs {(income * 0.10m):N0} / month");
            }
            else if (rate < 20)
            {
                result.Add("🎯 Target 20% monthly savings.");
                result.Add($"Goal: Rs {(income * 0.20m):N0} / month");
            }
            else
            {
                result.Add("🌟 Excellent savings rate! Consider long-term investments.");
            }

            result.Add($"🏠 Home Fund (5 years): Rs {(income * 60):N0}");
            result.Add($"🚗 Vehicle Fund (1 year): Rs {(income * 12):N0}");

            return result;
        }
    }
    #endregion

    #region Score Engine
    public interface IFinancialScoreEngine
    {
        int CalculateScore(IEnumerable<Transaction> tx, IEnumerable<Budget> budgets);
        string ScoreLabel(int score);
    }

    public class FinancialScoreEngine : IFinancialScoreEngine
    {
        public int CalculateScore(IEnumerable<Transaction> tx, IEnumerable<Budget> budgets)
        {
            var recent = tx.Where(t => t.Date >= DateTime.Now.AddMonths(-3)).ToList();
            if (!recent.Any()) return 0;

            int score = 0;

            score += ScoreSavingsRate(recent);
            score += ScoreDiversity(recent);
            score += ScoreConsistency(recent);

            if (budgets.Any()) score += 20;

            return score;
        }

        public string ScoreLabel(int score)
        {
            return score switch
            {
                >= 80 => $"{score}/100 - Excellent 🌟",
                >= 60 => $"{score}/100 - Good 👍",
                >= 40 => $"{score}/100 - Fair ⚖️",
                >= 20 => $"{score}/100 - Needs Improvement 📈",
                _ => $"{score}/100 - Getting Started 🌱"
            };
        }

        private int ScoreSavingsRate(List<Transaction> recent)
        {
            var income = recent.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var exp = recent.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            if (income <= 0) return 0;

            var rate = ((income - exp) / income) * 100;

            return rate switch
            {
                >= 20 => 40,
                >= 10 => 25,
                >= 0 => 10,
                _ => 0
            };
        }

        private int ScoreDiversity(List<Transaction> recent)
        {
            int c = recent.Where(t => t.Amount < 0).Select(t => t.Category).Distinct().Count();
            return c switch
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
    #endregion

    #region Prediction Engine
    public interface IPredictionEngine
    {
        List<string> Predict(IEnumerable<Transaction> tx);
    }

    public class PredictionEngine : IPredictionEngine
    {
        public List<string> Predict(IEnumerable<Transaction> tx)
        {
            var insights = new List<string>();

            var last3 = tx.Where(t => t.Date >= DateTime.Now.AddMonths(-3)).ToList();

            var monthGroups = last3
                .Where(t => t.Amount < 0)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new { g.Key, Amount = g.Sum(t => Math.Abs(t.Amount)) })
                .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
                .ToList();

            if (monthGroups.Count >= 2)
            {
                var avg = monthGroups.Average(x => x.Amount);
                var last = monthGroups.Last().Amount;

                insights.Add($"📊 Monthly spending is {(last > avg ? "increasing" : "decreasing")}.");
                insights.Add($"📅 Predicted next month: Rs {last:N0}");

                var topCats = last3
                    .Where(t => t.Amount < 0)
                    .GroupBy(t => t.Category)
                    .Select(g => new { g.Key, Avg = g.Sum(x => Math.Abs(x.Amount)) / monthGroups.Count })
                    .OrderByDescending(x => x.Avg)
                    .Take(3);

                foreach (var c in topCats)
                    insights.Add($"🏷 {c.Key}: expected Rs {c.Avg:N0}/month");
            }

            // Seasonal pattern
            int m = DateTime.Now.Month;

            var seasonal = tx
                .Where(t => t.Date.Month == m && t.Amount < 0)
                .GroupBy(t => t.Date.Year)
                .Select(g => g.Sum(t => Math.Abs(t.Amount)))
                .ToList();

            if (seasonal.Count > 1)
                insights.Add($"📅 Seasonal pattern for {DateTime.Now:MMMM}: Rs {seasonal.Average():N0}");

            return insights;
        }
    }
    #endregion
}

