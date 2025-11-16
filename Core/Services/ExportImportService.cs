using System;
using System.Collections.Generic;
using System.Globalization; // Added for robust number/date parsing
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks; // Added for async operations
using System.Windows.Forms;
using FinanceML.Core.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// Service for handling data export (CSV, JSON, PDF) and import (CSV, JSON) operations.
    /// This implementation follows the Singleton pattern.
    /// </summary>
    public class ExportImportService
    {
        private static ExportImportService? _instance;
        private readonly DataService _dataService;
        // Assume UserService is accessible and provides necessary user data
        // private readonly UserService _userService; // Declared but not used in the original. Let's assume it exists.

        /// <summary>
        /// Gets the singleton instance of the ExportImportService.
        /// </summary>
        public static ExportImportService Instance => _instance ??= new ExportImportService();

        /// <summary>
        /// Private constructor for Singleton pattern.
        /// </summary>
        private ExportImportService()
        {
            _dataService = DataService.Instance;
            // _userService = UserService.Instance; // Assuming this is correct
        }

        // --- EXPORT METHODS ---

        /// <summary>
        /// Exports all transactions to a CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>True if export was successful, otherwise false.</returns>
        public bool ExportToCSV(string filePath)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions();
                var csv = new StringBuilder();

                // Header
                csv.AppendLine("Date,Description,Category,Amount,Type");

                // Data - Use a safe format (ISO date, escaped description)
                foreach (var transaction in transactions.OrderBy(t => t.Date))
                {
                    // Ensure proper CSV escaping for descriptions that might contain commas
                    var escapedDescription = $"\"{transaction.Description.Replace("\"", "\"\"")}\"";
                    
                    csv.AppendLine($"{transaction.Date:yyyy-MM-dd}," +
                                   $"{escapedDescription}," +
                                   $"{transaction.Category}," +
                                   $"{transaction.Amount.ToString(CultureInfo.InvariantCulture)}," + // Use InvariantCulture for decimal separation
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

        /// <summary>
        /// Exports all data (Transactions, Budgets, User) to a JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>True if export was successful, otherwise false.</returns>
        public bool ExportToJSON(string filePath)
        {
            try
            {
                var data = new
                {
                    ExportDate = DateTime.Now,
                    Transactions = _dataService.GetAllTransactions().OrderBy(t => t.Date).ToList(), // Added ordering
                    Budgets = _dataService.GetAllBudgets(),
                    User = UserService.Instance.CurrentUser // Assuming UserService is available
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = true // Minor robustness improvement
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
        
        /// <summary>
        /// Exports a detailed financial report to a PDF file for a given date range.
        /// Consolidated the two PDF methods into one robust, modern-looking report.
        /// </summary>
        public bool ExportToPDF(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = _dataService.GetAllTransactions()
                    .Where(t => t.Date >= startDate.Date && t.Date <= endDate.Date.AddDays(1).AddTicks(-1)) // Adjusted date range for full day inclusion
                    .OrderBy(t => t.Date)
                    .ToList();

                // Calculate summary data directly
                var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                var netAmount = totalIncome - totalExpenses;
                
                var categoryBreakdown = transactions
                    .Where(t => t.Type == TransactionType.Expense) // Focus breakdown on expenses
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                // Calculate monthly trends
                var monthlyTrends = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                        TransactionCount = g.Count()
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

                // Create PDF document
                var document = new Document(PageSize.A4, 40, 40, 60, 60);
                // Use using statement for auto-disposal and safety
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Define fonts and colors - Improved color palette/consistency
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, new BaseColor(25, 130, 250)); // Bright Blue
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
                    var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.GRAY);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                    var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.LIGHT_GRAY);

                    // Header with logo and title
                    var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                    headerTable.SetWidths(new float[] { 1, 3 }); // Adjust width for better title space

                    // Logo cell (using emoji as placeholder - keep as is for compatibility)
                    var logoCell = new PdfPCell(new Phrase("📊", FontFactory.GetFont(FontFactory.HELVETICA, 36)))
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
                    titleCell.AddElement(new Paragraph("FinanceML Financial Report", titleFont));
                    titleCell.AddElement(new Paragraph($"Period: {startDate:MMM dd, yyyy} - {endDate:MMM dd, yyyy}", subHeaderFont));
                    titleCell.AddElement(new Paragraph($"Generated: {DateTime.Now:MMM dd, yyyy 'at' HH:mm}", smallFont));
                    headerTable.AddCell(titleCell);

                    document.Add(headerTable);
                    document.Add(new Paragraph(" ")); // Spacer
                    document.Add(new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -1)); // Divider
                    document.Add(new Paragraph(" ")); // Spacer

                    // Executive Summary Section
                    document.Add(new Paragraph("💰 Executive Summary", headerFont));
                    document.Add(new Paragraph(" ", smallFont)); // Small spacer

                    // Summary metrics in a grid
                    var metricsTable = new PdfPTable(3) { WidthPercentage = 100 }; // Changed to 3 columns
                    metricsTable.SetWidths(new float[] { 1, 1, 1 });

                    // Metric cards
                    AddMetricCard(metricsTable, "Total Income", $"Rs {totalIncome:N2}", new BaseColor(16, 185, 129)); // Green
                    AddMetricCard(metricsTable, "Total Expenses", $"Rs {totalExpenses:N2}", new BaseColor(239, 68, 68)); // Red
                    AddMetricCard(metricsTable, "Net Income", $"Rs {netAmount:N2}", 
                        netAmount >= 0 ? new BaseColor(25, 130, 250) : new BaseColor(239, 68, 68)); // Blue/Red based on result

                    document.Add(metricsTable);
                    document.Add(new Paragraph(" ")); // Spacer

                    // Category Breakdown Section
                    if (categoryBreakdown.Any())
                    {
                        document.Add(new Paragraph("📉 Top Expense Categories", headerFont));
                        document.Add(new Paragraph(" ", smallFont)); // Small spacer

                        var categoryTable = new PdfPTable(3) { WidthPercentage = 80 }; // Smaller table for visual appeal
                        categoryTable.HorizontalAlignment = Element.ALIGN_CENTER;
                        categoryTable.SetWidths(new float[] { 3, 2, 1 });

                        // Table headers
                        AddTableHeader(categoryTable, "Category");
                        AddTableHeader(categoryTable, "Amount");
                        AddTableHeader(categoryTable, "Percentage");

                        // Category data
                        foreach (var category in categoryBreakdown.Take(10))
                        {
                            var percentage = totalExpenses > 0 ? (category.Amount / totalExpenses) * 100 : 0;
                            
                            AddTableCell(categoryTable, GetCategoryIcon(category.Category) + " " + category.Category, normalFont);
                            AddTableCell(categoryTable, $"Rs {category.Amount:N2}", normalFont);
                            AddTableCell(categoryTable, $"{percentage:F1}%", normalFont);
                        }

                        document.Add(categoryTable);
                        document.Add(new Paragraph(" ")); // Spacer
                    }

                    // Monthly Trends Section
                    if (monthlyTrends.Any())
                    {
                        document.Add(new Paragraph("📈 Monthly Trends", headerFont));
                        document.Add(new Paragraph(" ", smallFont)); // Small spacer

                        var trendsTable = new PdfPTable(4) { WidthPercentage = 100 }; // Simplified to 4 columns
                        trendsTable.SetWidths(new float[] { 2, 2, 2, 2 });

                        // Table headers
                        AddTableHeader(trendsTable, "Month");
                        AddTableHeader(trendsTable, "Income");
                        AddTableHeader(trendsTable, "Expenses");
                        AddTableHeader(trendsTable, "Net");

                        // Monthly data
                        foreach (var month in monthlyTrends)
                        {
                            var monthName = new DateTime(month.Year, month.Month, 1).ToString("MMM yyyy");
                            var net = month.Income - month.Expenses;
                            
                            AddTableCell(trendsTable, monthName, normalFont);
                            AddTableCell(trendsTable, $"Rs {month.Income:N2}", normalFont, new BaseColor(16, 185, 129));
                            AddTableCell(trendsTable, $"Rs {month.Expenses:N2}", normalFont, new BaseColor(239, 68, 68));
                            AddTableCell(trendsTable, $"Rs {net:N2}", normalFont, 
                                net >= 0 ? new BaseColor(25, 130, 250) : new BaseColor(239, 68, 68));
                        }

                        document.Add(trendsTable);
                        document.Add(new Paragraph(" ")); // Spacer
                    }

                    // Recent Transactions Section
                    if (transactions.Any())
                    {
                        document.Add(new Paragraph("📋 All Transactions (Detailed List)", headerFont));
                        document.Add(new Paragraph(" ", smallFont)); // Small spacer

                        var transactionsTable = new PdfPTable(4) { WidthPercentage = 100 };
                        transactionsTable.SetWidths(new float[] { 2, 4, 2, 2 });

                        // Table headers
                        AddTableHeader(transactionsTable, "Date");
                        AddTableHeader(transactionsTable, "Description");
                        AddTableHeader(transactionsTable, "Category");
                        AddTableHeader(transactionsTable, "Amount");

                        // Transaction data 
                        foreach (var transaction in transactions)
                        {
                            AddTableCell(transactionsTable, transaction.Date.ToString("MMM dd, yyyy"), normalFont);
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
                    var footerParagraph = new Paragraph($"--- End of Report | Generated by FinanceML on {DateTime.Now:MMMM dd, yyyy 'at' HH:mm} ---", smallFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    document.Add(footerParagraph);

                    document.Close();
                } // Stream disposal handled by using
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // --- IMPORT METHODS ---

        /// <summary>
        /// Imports transactions from a CSV file. Includes improved parsing and duplicate checking.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>True if import was successful, otherwise false.</returns>
        public bool ImportFromCSV(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length <= 1) return false;

                var importedCount = 0;
                var skippedCount = 0;
                var existingTransactions = _dataService.GetAllTransactions().ToList(); // Cache existing transactions for faster lookup

                for (int i = 1; i < lines.Length; i++) // Skip header
                {
                    // Use the dedicated CSV parser
                    var parts = ParseCSVLine(lines[i]); 
                    if (parts.Length < 5)
                    {
                        skippedCount++;
                        continue;
                    }

                    try
                    {
                        // Use CultureInfo.InvariantCulture for parsing to match export consistency
                        var transaction = new Transaction
                        {
                            Date = DateTime.Parse(parts[0], CultureInfo.InvariantCulture),
                            Description = parts[1].Trim('"'),
                            Category = parts[2],
                            Amount = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                            Type = Enum.Parse<TransactionType>(parts[4], true) // 'true' for case-insensitive parsing
                        };

                        // Check for duplicates - using cached list
                        var existing = existingTransactions
                            .Any(t => t.Date.Date == transaction.Date.Date &&
                                     string.Equals(t.Description, transaction.Description, StringComparison.OrdinalIgnoreCase) && // Case-insensitive compare
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
                        skippedCount++; // Skip line if parsing fails
                    }
                }
                
                // Refresh the data service state if needed (implementation dependent)
                // _dataService.SaveData(); 

                MessageBox.Show($"Import completed!\nImported: {importedCount} transactions\nSkipped: {skippedCount} invalid/duplicates",
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

        /// <summary>
        /// Imports transactions and budgets from a JSON file. Now uses a strongly-typed model for deserialization (Recommended).
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>True if import was successful, otherwise false.</returns>
        public bool ImportFromJSON(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                
                // Define a strongly-typed structure that matches the export format
                var importDataModel = new 
                { 
                    ExportDate = DateTime.Now, // Placeholder
                    Transactions = new List<Transaction>(),
                    Budgets = new List<Budget>(),
                    User = (object)null // Placeholder for user object
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    // Handle flexible date formats if the source JSON might be non-standard
                };

                // Use the strongly-typed anonymous object for deserialization
                var data = JsonSerializer.Deserialize(json, importDataModel.GetType(), options);

                // Reflectively extract the necessary properties
                var importedTransactions = data.GetType().GetProperty("Transactions")?.GetValue(data) as List<Transaction> ?? new List<Transaction>();
                var importedBudgets = data.GetType().GetProperty("Budgets")?.GetValue(data) as List<Budget> ?? new List<Budget>();

                var transactionsImportedCount = 0;
                var budgetsImportedCount = 0;
                var existingTransactions = _dataService.GetAllTransactions().ToList(); // Cache existing
                var existingBudgets = _dataService.GetAllBudgets().ToList(); // Cache existing

                // Import transactions
                foreach (var transaction in importedTransactions)
                {
                    // Check for duplicates
                    var existing = existingTransactions
                        .Any(t => t.Date.Date == transaction.Date.Date &&
                                 string.Equals(t.Description, transaction.Description, StringComparison.OrdinalIgnoreCase) &&
                                 t.Amount == transaction.Amount);

                    if (!existing)
                    {
                        _dataService.AddTransaction(transaction);
                        transactionsImportedCount++;
                    }
                }

                // Import budgets
                foreach (var budget in importedBudgets)
                {
                    // Check for duplicates
                    var existing = existingBudgets
                        .Any(b => b.Name == budget.Name && b.Category == budget.Category);

                    if (!existing)
                    {
                        _dataService.AddBudget(budget);
                        budgetsImportedCount++;
                    }
                }

                MessageBox.Show($"Import completed!\nTransactions Imported: {transactionsImportedCount}\nBudgets Imported: {budgetsImportedCount}",
                                "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                // Better error handling/reporting for JSON structure issues
                MessageBox.Show($"Error importing from JSON. Ensure the file structure is correct: {ex.Message}", "Import Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        
        // --- UTILITY METHODS ---

        /// <summary>
        /// Improved CSV line parser that correctly handles quoted fields containing commas or escaped quotes.
        /// </summary>
        /// <param name="line">The CSV line string.</param>
        /// <returns>An array of field strings.</returns>
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
                    // Handle escaped quotes "" within a quoted field
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // Skip the next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
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

        // --- PDF HELPER METHODS (Improved) ---

        // Consolidated and simplified PDF helper methods. The old, less-featured `ExportReport` and `ExportToSimplePDF` methods
        // are removed/consolidated into the single, robust `ExportToPDF` above to reduce redundancy and maintenance.

        /// <summary>
        /// Adds a metric card (cell) to a PDF table.
        /// </summary>
        private void AddMetricCard(PdfPTable table, string title, string value, BaseColor color)
        {
            var cell = new PdfPCell();
            cell.Border = Rectangle.BOX;
            cell.BorderColor = new BaseColor(229, 231, 235);
            cell.BorderWidth = 1;
            cell.Padding = 10;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(250, 250, 250); // Light background for contrast

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(107, 114, 128));
            var valueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, color);

            cell.AddElement(new Paragraph(title, titleFont) { Alignment = Element.ALIGN_CENTER });
            cell.AddElement(new Paragraph(" ", titleFont)); // Small spacer
            cell.AddElement(new Paragraph(value, valueFont) { Alignment = Element.ALIGN_CENTER });

            table.AddCell(cell);
        }

        /// <summary>
        /// Adds a styled table header cell to a PDF table.
        /// </summary>
        private void AddTableHeader(PdfPTable table, string text)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var cell = new PdfPCell(new Phrase(text, headerFont))
            {
                BackgroundColor = new BaseColor(40, 50, 60), // Darker header for contrast
                Border = Rectangle.NO_BORDER,
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_CENTER // Center alignment for headers
            };
            table.AddCell(cell);
        }

        /// <summary>
        /// Adds a data cell to a PDF table with optional text color.
        /// </summary>
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

        /// <summary>
        /// Provides an emoji icon for common categories.
        /// </summary>
        private string GetCategoryIcon(string category)
        {
            return category.ToLower() switch
            {
                var c when c.Contains("food") || c.Contains("dining") => "🍽️",
                var c when c.Contains("transport") || c.Contains("car") => "🚗",
                var c when c.Contains("shopping") || c.Contains("retail") => "🛍️",
                var c when c.Contains("entertainment") || c.Contains("fun") => "🎬",
                var c when c.Contains("utilities") || c.Contains("bills") || c.Contains("rent") => "🏠", // Changed from lightbulb to house
                var c when c.Contains("healthcare") || c.Contains("medical") => "🏥",
                var c when c.Contains("education") || c.Contains("school") => "📚",
                var c when c.Contains("travel") || c.Contains("vacation") => "✈️",
                var c when c.Contains("income") || c.Contains("salary") => "💰",
                var c when c.Contains("investment") || c.Contains("stock") => "📈", // Added new category
                _ => "📦"
            };
        }
    }
}
