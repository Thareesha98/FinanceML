using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using FinanceML.Core.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;

namespace FinanceML.Core.Services
{
    public class ExportImportService
    {
        private static ExportImportService? _instance;
        private readonly DataService _dataService;

        public static ExportImportService Instance => _instance ??= new ExportImportService();

        private ExportImportService()
        {
            _dataService = DataService.Instance;
        }

        public bool ExportToCSV(string filePath)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions();
                var csv = new StringBuilder();

                // Header
                csv.AppendLine("Date,Description,Category,Amount,Type");

                // Data
                foreach (var transaction in transactions.OrderBy(t => t.Date))
                {
                    csv.AppendLine($"{transaction.Date:yyyy-MM-dd}," +
                                  $"\"{transaction.Description}\"," +
                                  $"{transaction.Category}," +
                                  $"{transaction.Amount}," +
                                  $"{transaction.Type}");
                }

                File.WriteAllText(filePath, csv.ToString());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Export Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ExportToJSON(string filePath)
        {
            try
            {
                var data = new
                {
                    ExportDate = DateTime.Now,
                    Transactions = _dataService.GetAllTransactions(),
                    Budgets = _dataService.GetAllBudgets(),
                    User = UserService.Instance.CurrentUser
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to JSON: {ex.Message}", "Export Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ImportFromCSV(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length <= 1) return false;

                var importedCount = 0;
                var skippedCount = 0;

                for (int i = 1; i < lines.Length; i++) // Skip header
                {
                    var parts = ParseCSVLine(lines[i]);
                    if (parts.Length >= 5)
                    {
                        try
                        {
                            var transaction = new Transaction
                            {
                                Date = DateTime.Parse(parts[0]),
                                Description = parts[1].Trim('"'),
                                Category = parts[2],
                                Amount = decimal.Parse(parts[3]),
                                Type = Enum.Parse<TransactionType>(parts[4])
                            };

                            // Check for duplicates
                            var existing = _dataService.GetAllTransactions()
                                .Any(t => t.Date.Date == transaction.Date.Date &&
                                         t.Description == transaction.Description &&
                                         t.Amount == transaction.Amount);

                            if (!existing)
                            {
                                _dataService.AddTransaction(transaction);
                                importedCount++;
                            }
                            else
                            {
                                skippedCount++;
                            }
                        }
                        catch
                        {
                            skippedCount++;
                        }
                    }
                }

                MessageBox.Show($"Import completed!\nImported: {importedCount} transactions\nSkipped: {skippedCount} duplicates",
                               "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing from CSV: {ex.Message}", "Import Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ImportFromJSON(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                var importedTransactions = 0;
                var importedBudgets = 0;

                // Import transactions
                if (data.TryGetProperty("transactions", out var transactionsElement))
                {
                    foreach (var transactionElement in transactionsElement.EnumerateArray())
                    {
                        try
                        {
                            var transaction = new Transaction
                            {
                                Date = transactionElement.GetProperty("date").GetDateTime(),
                                Description = transactionElement.GetProperty("description").GetString() ?? "",
                                Category = transactionElement.GetProperty("category").GetString() ?? "",
                                Amount = transactionElement.GetProperty("amount").GetDecimal(),
                                Type = Enum.Parse<TransactionType>(transactionElement.GetProperty("type").GetString() ?? "Expense")
                            };

                            // Check for duplicates
                            var existing = _dataService.GetAllTransactions()
                                .Any(t => t.Date.Date == transaction.Date.Date &&
                                         t.Description == transaction.Description &&
                                         t.Amount == transaction.Amount);

                            if (!existing)
                            {
                                _dataService.AddTransaction(transaction);
                                importedTransactions++;
                            }
                        }
                        catch { /* Skip invalid transactions */ }
                    }
                }

                // Import budgets
                if (data.TryGetProperty("budgets", out var budgetsElement))
                {
                    foreach (var budgetElement in budgetsElement.EnumerateArray())
                    {
                        try
                        {
                            var budget = new Budget
                            {
                                Name = budgetElement.GetProperty("name").GetString() ?? "",
                                Category = budgetElement.GetProperty("category").GetString() ?? "",
                                Amount = budgetElement.GetProperty("amount").GetDecimal(),
                                Period = Enum.Parse<BudgetPeriod>(budgetElement.GetProperty("period").GetString() ?? "Monthly"),
                                StartDate = budgetElement.GetProperty("startDate").GetDateTime(),
                                EndDate = budgetElement.GetProperty("endDate").GetDateTime(),
                                IsActive = budgetElement.GetProperty("isActive").GetBoolean()
                            };

                            // Check for duplicates
                            var existing = _dataService.GetAllBudgets()
                                .Any(b => b.Name == budget.Name && b.Category == budget.Category);

                            if (!existing)
                            {
                                _dataService.AddBudget(budget);
                                importedBudgets++;
                            }
                        }
                        catch { /* Skip invalid budgets */ }
                    }
                }

                MessageBox.Show($"Import completed!\nTransactions: {importedTransactions}\nBudgets: {importedBudgets}",
                               "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing from JSON: {ex.Message}", "Import Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool ExportToPDF(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions()
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .OrderBy(t => t.Date)
                    .ToList();

                // Calculate summary data directly
                var totalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
                var totalExpenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
                var netAmount = totalIncome - totalExpenses;
                
                var categoryBreakdown = transactions
                    .Where(t => t.Amount < 0)
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => Math.Abs(t.Amount)) })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                // Calculate monthly trends
                var monthlyTrends = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Income = g.Where(t => t.Amount > 0).Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount)),
                        TransactionCount = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

                // Create PDF document
                var document = new Document(PageSize.A4, 40, 40, 60, 60);
                var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                
                document.Open();

                // Define fonts and colors
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, new BaseColor(37, 99, 235));
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(31, 41, 55));
                var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(75, 85, 99));
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, new BaseColor(107, 114, 128));

                // Header with logo and title
                var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                headerTable.SetWidths(new float[] { 1, 2 });

                // Logo cell (using emoji as placeholder)
                var logoCell = new PdfPCell(new Phrase("💰", FontFactory.GetFont(FontFactory.HELVETICA, 36)))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingBottom = 10
                };
                headerTable.AddCell(logoCell);

                // Title cell
                var titleCell = new PdfPCell();
                titleCell.Border = Rectangle.NO_BORDER;
                titleCell.AddElement(new Paragraph("FinanceML", titleFont));
                titleCell.AddElement(new Paragraph("Financial Report", headerFont));
                titleCell.AddElement(new Paragraph($"Period: {startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}", normalFont));
                titleCell.AddElement(new Paragraph($"Generated: {DateTime.Now:MMM dd, yyyy 'at' HH:mm}", smallFont));
                headerTable.AddCell(titleCell);

                document.Add(headerTable);
                document.Add(new Paragraph(" ")); // Spacer

                // Executive Summary Section
                var summarySection = new PdfPTable(1) { WidthPercentage = 100 };
                var summaryHeaderCell = new PdfPCell(new Phrase("📊 Executive Summary", headerFont))
                {
                    BackgroundColor = new BaseColor(249, 250, 251),
                    Border = Rectangle.NO_BORDER,
                    Padding = 10,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                summarySection.AddCell(summaryHeaderCell);

                // Summary metrics in a grid
                var metricsTable = new PdfPTable(4) { WidthPercentage = 100 };
                metricsTable.SetWidths(new float[] { 1, 1, 1, 1 });

                // Metric cards
                var savingsRate = totalIncome > 0 ? ((netAmount / totalIncome) * 100) : 0;
                AddMetricCard(metricsTable, "💰 Total Income", $"Rs {totalIncome:N2}", new BaseColor(16, 185, 129));
                AddMetricCard(metricsTable, "💸 Total Expenses", $"Rs {totalExpenses:N2}", new BaseColor(239, 68, 68));
                AddMetricCard(metricsTable, "📈 Net Income", $"Rs {netAmount:N2}", 
                    netAmount >= 0 ? new BaseColor(16, 185, 129) : new BaseColor(239, 68, 68));
                AddMetricCard(metricsTable, "💾 Savings Rate", $"{savingsRate:F1}%", new BaseColor(245, 158, 11));

                document.Add(summarySection);
                document.Add(metricsTable);
                document.Add(new Paragraph(" ")); // Spacer

                // Category Breakdown Section
                if (categoryBreakdown.Any())
                {
                    document.Add(new Paragraph("📊 Spending by Category", headerFont));
                    document.Add(new Paragraph(" ", smallFont)); // Small spacer

                    var categoryTable = new PdfPTable(4) { WidthPercentage = 100 };
                    categoryTable.SetWidths(new float[] { 3, 2, 1, 1 });

                    // Table headers
                    AddTableHeader(categoryTable, "Category");
                    AddTableHeader(categoryTable, "Amount");
                    AddTableHeader(categoryTable, "Percentage");
                    AddTableHeader(categoryTable, "Transactions");

                    // Category data
                    foreach (var category in categoryBreakdown.Take(10))
                    {
                        var percentage = totalExpenses > 0 ? (category.Amount / totalExpenses) * 100 : 0;
                        var transactionCount = transactions.Count(t => t.Category == category.Category && t.Amount < 0);
                        
                        AddTableCell(categoryTable, GetCategoryIcon(category.Category) + " " + category.Category, normalFont);
                        AddTableCell(categoryTable, $"Rs {category.Amount:N2}", normalFont);
                        AddTableCell(categoryTable, $"{percentage:F1}%", normalFont);
                        AddTableCell(categoryTable, transactionCount.ToString(), normalFont);
                    }

                    document.Add(categoryTable);
                    document.Add(new Paragraph(" ")); // Spacer
                }

                // Monthly Trends Section
                if (monthlyTrends.Any())
                {
                    document.Add(new Paragraph("📈 Monthly Trends", headerFont));
                    document.Add(new Paragraph(" ", smallFont)); // Small spacer

                    var trendsTable = new PdfPTable(5) { WidthPercentage = 100 };
                    trendsTable.SetWidths(new float[] { 2, 2, 2, 2, 1 });

                    // Table headers
                    AddTableHeader(trendsTable, "Month");
                    AddTableHeader(trendsTable, "Income");
                    AddTableHeader(trendsTable, "Expenses");
                    AddTableHeader(trendsTable, "Net");
                    AddTableHeader(trendsTable, "Transactions");

                    // Monthly data
                    foreach (var month in monthlyTrends.TakeLast(6))
                    {
                        var monthName = new DateTime(month.Year, month.Month, 1).ToString("MMM yyyy");
                        var net = month.Income - month.Expenses;
                        
                        AddTableCell(trendsTable, monthName, normalFont);
                        AddTableCell(trendsTable, $"Rs {month.Income:N2}", normalFont);
                        AddTableCell(trendsTable, $"Rs {month.Expenses:N2}", normalFont);
                        AddTableCell(trendsTable, $"Rs {net:N2}", normalFont, 
                            net >= 0 ? new BaseColor(16, 185, 129) : new BaseColor(239, 68, 68));
                        AddTableCell(trendsTable, month.TransactionCount.ToString(), normalFont);
                    }

                    document.Add(trendsTable);
                    document.Add(new Paragraph(" ")); // Spacer
                }

                // Recent Transactions Section
                if (transactions.Any())
                {
                    document.Add(new Paragraph("📋 Recent Transactions", headerFont));
                    document.Add(new Paragraph(" ", smallFont)); // Small spacer

                    var transactionsTable = new PdfPTable(4) { WidthPercentage = 100 };
                    transactionsTable.SetWidths(new float[] { 2, 4, 2, 2 });

                    // Table headers
                    AddTableHeader(transactionsTable, "Date");
                    AddTableHeader(transactionsTable, "Description");
                    AddTableHeader(transactionsTable, "Category");
                    AddTableHeader(transactionsTable, "Amount");

                    // Transaction data (last 20 transactions)
                    foreach (var transaction in transactions.TakeLast(20))
                    {
                        AddTableCell(transactionsTable, transaction.Date.ToString("MMM dd"), normalFont);
                        AddTableCell(transactionsTable, transaction.Description, normalFont);
                        AddTableCell(transactionsTable, transaction.Category, normalFont);
                        
                        var amountColor = transaction.Type == TransactionType.Income ? 
                            new BaseColor(16, 185, 129) : new BaseColor(239, 68, 68);
                        var amountText = transaction.Type == TransactionType.Income ? 
                            $"+Rs {transaction.Amount:N2}" : $"-Rs {transaction.Amount:N2}";
                        
                        AddTableCell(transactionsTable, amountText, normalFont, amountColor);
                    }

                    document.Add(transactionsTable);
                }

                // Footer
                document.Add(new Paragraph(" ")); // Spacer
                var footerParagraph = new Paragraph($"Report generated by FinanceML on {DateTime.Now:MMMM dd, yyyy 'at' HH:mm}", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(footerParagraph);

                document.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void AddMetricCard(PdfPTable table, string title, string value, BaseColor color)
        {
            var cell = new PdfPCell();
            cell.Border = Rectangle.BOX;
            cell.BorderColor = new BaseColor(229, 231, 235);
            cell.BorderWidth = 1;
            cell.Padding = 10;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, new BaseColor(107, 114, 128));
            var valueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, color);

            cell.AddElement(new Paragraph(title, titleFont) { Alignment = Element.ALIGN_CENTER });
            cell.AddElement(new Paragraph(" ", titleFont)); // Small spacer
            cell.AddElement(new Paragraph(value, valueFont) { Alignment = Element.ALIGN_CENTER });

            table.AddCell(cell);
        }

        private void AddTableHeader(PdfPTable table, string text)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var cell = new PdfPCell(new Phrase(text, headerFont))
            {
                BackgroundColor = new BaseColor(31, 41, 55),
                Border = Rectangle.NO_BORDER,
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.AddCell(cell);
        }

        private void AddTableCell(PdfPTable table, string text, iTextSharp.text.Font font, BaseColor? textColor = null)
        {
            var cellFont = textColor != null ? 
                FontFactory.GetFont(font.Familyname, font.Size, textColor) : font;
            
            var cell = new PdfPCell(new Phrase(text, cellFont))
            {
                Border = Rectangle.BOTTOM_BORDER,
                BorderColor = new BaseColor(229, 231, 235),
                BorderWidth = 0.5f,
                Padding = 6,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            table.AddCell(cell);
        }

        private string GetCategoryIcon(string category)
        {
            return category.ToLower() switch
            {
                var c when c.Contains("food") || c.Contains("dining") => "🍽️",
                var c when c.Contains("transport") => "🚗",
                var c when c.Contains("shopping") => "🛍️",
                var c when c.Contains("entertainment") => "🎬",
                var c when c.Contains("utilities") || c.Contains("bills") => "💡",
                var c when c.Contains("healthcare") || c.Contains("medical") => "🏥",
                var c when c.Contains("education") => "📚",
                var c when c.Contains("travel") => "✈️",
                var c when c.Contains("income") => "💰",
                _ => "📦"
            };
        }

        public bool ExportReport(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions()
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .OrderBy(t => t.Date)
                    .ToList();

                var report = new StringBuilder();
                report.AppendLine("FINANCIAL REPORT");
                report.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                report.AppendLine(new string('=', 50));
                report.AppendLine();

                // Summary
                var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                var netIncome = totalIncome - totalExpenses;

                report.AppendLine("SUMMARY");
                report.AppendLine($"Total Income:  Rs {totalIncome:F2}");
                report.AppendLine($"Total Expenses: Rs {totalExpenses:F2}");
                report.AppendLine($"Net Income:    Rs {netIncome:F2}");
                report.AppendLine();

                // Income by category
                var incomeByCategory = transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .GroupBy(t => t.Category)
                    .OrderByDescending(g => g.Sum(t => t.Amount))
                    .ToList();

                if (incomeByCategory.Any())
                {
                    report.AppendLine("INCOME BY CATEGORY");
                    foreach (var group in incomeByCategory)
                    {
                        report.AppendLine($"{group.Key}: Rs {group.Sum(t => t.Amount):F2}");
                    }
                    report.AppendLine();
                }

                // Expenses by category
                var expensesByCategory = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .GroupBy(t => t.Category)
                    .OrderByDescending(g => g.Sum(t => t.Amount))
                    .ToList();

                if (expensesByCategory.Any())
                {
                    report.AppendLine("EXPENSES BY CATEGORY");
                    foreach (var group in expensesByCategory)
                    {
                        var percentage = totalExpenses > 0 ? (group.Sum(t => t.Amount) / totalExpenses) * 100 : 0;
                        report.AppendLine($"{group.Key}: Rs {group.Sum(t => t.Amount):F2} ({percentage:F1}%)");
                    }
                    report.AppendLine();
                }

                // Monthly breakdown
                var monthlyData = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToList();

                if (monthlyData.Any())
                {
                    report.AppendLine("MONTHLY BREAKDOWN");
                    foreach (var month in monthlyData)
                    {
                        var monthIncome = month.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                        var monthExpenses = month.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                        report.AppendLine($"{month.Key.Year}-{month.Key.Month:D2}: Income Rs {monthIncome:F2}, Expenses Rs {monthExpenses:F2}, Net Rs {monthIncome - monthExpenses:F2}");
                    }
                    report.AppendLine();
                }

                // All transactions
                report.AppendLine("TRANSACTION DETAILS");
                report.AppendLine("Date       | Description                    | Category      | Amount    | Type");
                report.AppendLine(new string('-', 80));
                
                foreach (var transaction in transactions)
                {
                    report.AppendLine($"{transaction.Date:yyyy-MM-dd} | {transaction.Description,-30} | {transaction.Category,-13} | Rs {transaction.Amount,8:F2} | {transaction.Type}");
                }

                File.WriteAllText(filePath, report.ToString());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Export Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string[] ParseCSVLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }

        public bool ExportToSimplePDF(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions()
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .OrderBy(t => t.Date)
                    .ToList();

                // Create simple PDF document
                var document = new Document(PageSize.A4, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                
                document.Open();

                // Simple fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                // Title
                document.Add(new Paragraph("FinanceML Financial Report", titleFont));
                document.Add(new Paragraph($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", normalFont));
                document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", normalFont));
                document.Add(new Paragraph(" ")); // Spacer

                // Summary
                var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                var netIncome = totalIncome - totalExpenses;

                document.Add(new Paragraph("Financial Summary", headerFont));
                document.Add(new Paragraph($"Total Income: Rs {totalIncome:F2}", normalFont));
                document.Add(new Paragraph($"Total Expenses: Rs {totalExpenses:F2}", normalFont));
                document.Add(new Paragraph($"Net Income: Rs {netIncome:F2}", normalFont));
                document.Add(new Paragraph(" ")); // Spacer

                // Category breakdown
                var categoryBreakdown = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Amount)
                    .Take(10);

                if (categoryBreakdown.Any())
                {
                    document.Add(new Paragraph("Top Expense Categories", headerFont));
                    foreach (var category in categoryBreakdown)
                    {
                        var percentage = totalExpenses > 0 ? (category.Amount / totalExpenses) * 100 : 0;
                        document.Add(new Paragraph($"{category.Category}: Rs {category.Amount:F2} ({percentage:F1}%)", normalFont));
                    }
                    document.Add(new Paragraph(" ")); // Spacer
                }

                // Recent transactions
                document.Add(new Paragraph("Recent Transactions", headerFont));
                foreach (var transaction in transactions.TakeLast(20))
                {
                    var type = transaction.Type == TransactionType.Income ? "+" : "-";
                    document.Add(new Paragraph($"{transaction.Date:yyyy-MM-dd} | {transaction.Description} | {transaction.Category} | {type}Rs {transaction.Amount:F2}", normalFont));
                }

                document.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
