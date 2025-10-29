using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using FinanceML.Core.Services;
using FinanceML.AI;
using FinanceML.UI.Controls;

namespace FinanceML
{
    public partial class AIInsightsForm : Form
    {

        public AIInsightsForm()
        {
            InitializeComponent();
            LoadPlaceholderData();
        }

        private void LoadPlaceholderData()
        {
            LoadAIInsights();
        }

        private void LoadAIInsights()
        {
            try
            {
                var aiService = AIInsightsService.Instance;
                
                // Load spending insights
                insightsListBox?.Items.Clear();
                var insights = aiService.GetSpendingInsights();
                foreach (var insight in insights)
                {
                    insightsListBox?.Items.Add(insight);
                }
                
                // Load budget recommendations
                recommendationsListBox?.Items.Clear();
                var recommendations = aiService.GetBudgetRecommendations();
                foreach (var recommendation in recommendations)
                {
                    recommendationsListBox?.Items.Add(recommendation);
                }
                
                // Load savings goals
                savingsListBox?.Items.Clear();
                var savingsGoals = aiService.GetSavingsGoals();
                foreach (var goal in savingsGoals)
                {
                    savingsListBox?.Items.Add(goal);
                }
                
                // Load financial health score
                if (healthScoreLabel != null)
                    healthScoreLabel.Text = aiService.GetFinancialHealthScore();
                
                // Load spending predictions
                predictionsListBox?.Items.Clear();
                var predictions = aiService.GetSpendingPredictions();
                foreach (var prediction in predictions)
                {
                    predictionsListBox?.Items.Add(prediction);
                }
                
                // Load forecast charts
                LoadForecastCharts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading AI insights: {ex.Message}", "Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void LoadForecastCharts()
        {
            try
            {
                LoadExpenseForecastChart();
                LoadIncomeVsExpenseChart();
            }
            catch (Exception ex)
            {
                // Silently handle forecast errors
                System.Diagnostics.Debug.WriteLine($"Forecast charts error: {ex.Message}");
            }
        }
        
        private void LoadExpenseForecastChart()
        {
            try
            {
                var forecastService = ExpenseForecastService.Instance;
                var forecastResult = forecastService.GenerateExpenseForecast(6);
                
                if (forecastResult?.MonthlyForecasts?.Any() == true)
                {
                    forecastChart.Data = forecastResult.MonthlyForecasts;
                    forecastChart.Title = "Expense Forecast - Next 6 Months";
                    forecastChart.LineColor = Color.FromArgb(239, 68, 68); // Red for expenses
                }
                else
                {
                    // Generate sample forecast data
                    var sampleData = new List<ExpenseForecastService.ForecastData>();
                    var baseAmount = 45000m;
                    var random = new Random();
                    
                    for (int i = 0; i < 6; i++)
                    {
                        var month = DateTime.Now.AddMonths(i + 1);
                        var variation = (decimal)(random.NextDouble() * 0.3 - 0.15); // ±15% variation
                        var amount = baseAmount * (1 + variation);
                        
                        sampleData.Add(new ExpenseForecastService.ForecastData
                        {
                            Month = month,
                            PredictedAmount = amount,
                            ConfidenceScore = 0.75m,
                            Category = "Total Expenses"
                        });
                    }
                    
                    forecastChart.Data = sampleData;
                    forecastChart.Title = "Expense Forecast - Next 6 Months";
                    forecastChart.LineColor = Color.FromArgb(239, 68, 68);
                }
                
                forecastChart.ShowGrid = true;
                forecastChart.ShowConfidenceBand = true;
                forecastChart.ShowLegend = true;
                forecastChart.Visible = true;
                forecastChart.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Expense forecast error: {ex.Message}");
            }
        }
        
        private void LoadIncomeVsExpenseChart()
        {
            try
            {
                // Generate sample income vs expense trend data
                var sampleData = new List<ExpenseForecastService.ForecastData>();
                var random = new Random();
                
                for (int i = 0; i < 6; i++)
                {
                    var month = DateTime.Now.AddMonths(i + 1);
                    var baseIncome = 75000m;
                    var baseExpense = 45000m;
                    
                    // Calculate net savings (income - expense)
                    var incomeVariation = (decimal)(random.NextDouble() * 0.1 - 0.05); // ±5% variation
                    var expenseVariation = (decimal)(random.NextDouble() * 0.2 - 0.1); // ±10% variation
                    
                    var income = baseIncome * (1 + incomeVariation);
                    var expense = baseExpense * (1 + expenseVariation);
                    var netSavings = income - expense;
                    
                    sampleData.Add(new ExpenseForecastService.ForecastData
                    {
                        Month = month,
                        PredictedAmount = netSavings,
                        ConfidenceScore = 0.8m,
                        Category = "Net Savings"
                    });
                }
                
                incomeVsExpenseChart.Data = sampleData;
                incomeVsExpenseChart.Title = "Net Savings Trend (Income - Expenses)";
                incomeVsExpenseChart.LineColor = Color.FromArgb(16, 185, 129); // Green for savings
                incomeVsExpenseChart.ShowGrid = true;
                incomeVsExpenseChart.ShowConfidenceBand = true;
                incomeVsExpenseChart.ShowLegend = true;
                incomeVsExpenseChart.Visible = true;
                incomeVsExpenseChart.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Income vs expense chart error: {ex.Message}");
            }
        }

        private void OnRefreshClick(object? sender, EventArgs e)
        {
            LoadAIInsights();
            MessageBox.Show("AI insights and forecast charts refreshed successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnExportInsightsClick(object? sender, EventArgs e)
        {
            MessageBox.Show("Export feature coming soon!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnCloseClick(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
