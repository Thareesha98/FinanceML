using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceML.Core.Models;
using FinanceML.Core.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// Handles exporting and importing of CSV, JSON, and PDF reports.
    /// Completely rewritten for DI, async usage, testability, and clean error handling.
    /// </summary>
    public class ExportImportService : IExportImportService
    {
        private readonly IDataService _dataService;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor intended for dependency injection.
        /// </summary>
        public ExportImportService(IDataService dataService, IUserService userService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // ------------------------------------------------------------
        // EXPORT METHODS (CSV / JSON / PDF)
        // ------------------------------------------------------------

        public async Task<Result> ExportToCsvAsync(string filePath)
        {
            try
            {
                var transactions = await _dataService.GetAllTransactionsAsync();
                var sorted = transactions.OrderBy(t => t.Date).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("Date,Description,Category,Amount,Type");

                foreach (var t in sorted)
                {
                    var desc = $"\"{t.Description.Replace("\"", "\"\"")}\"";

                    sb.AppendLine(
                        $"{t.Date:yyyy-MM-dd}," +
                        $"{desc}," +
                        $"{t.Category}," +
                        $"{t.Amount.ToString(CultureInfo.InvariantCulture)}," +
                        $"{t.Type}"
                    );
                }

                await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"CSV export failed: {ex.Message}");
            }
        }

        public async Task<Result> ExportToJsonAsync(string filePath)
        {
            try
            {
                var payload = new ExportPayload
                {
                    ExportDate = DateTime.Now,
                    Transactions = (await _dataService.GetAllTransactionsAsync()).OrderBy(t => t.Date).ToList(),
                    Budgets = await _dataService.GetAllBudgetsAsync(),
                    User = await _userService.GetCurrentUserAsync()
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(filePath, json);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"JSON export failed: {ex.Message}");
            }
        }

        public async Task<Result> ExportToPdfAsync(string filePath, DateTime start, DateTime end)
        {
            try
            {
                var transactions = (await _dataService.GetAllTransactionsAsync())
                    .Where(t => t.Date >= start && t.Date <= end.AddDays(1).AddTicks(-1))
                    .OrderBy(t => t.Date)
                    .ToList();

                var pdf = new PdfReportBuilder(transactions, start, end);
                pdf.Build(filePath);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"PDF export failed: {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // IMPORT METHODS (CSV / JSON)
        // ------------------------------------------------------------

        public async Task<Result> ImportFromCsvAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return Result.Failure("CSV file not found.");

                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length <= 1)
                    return Result.Failure("CSV is empty.");

                var existing = (await _dataService.GetAllTransactionsAsync()).ToList();
                int imported = 0;
                int skipped = 0;

                foreach (var line in lines.Skip(1))
                {
                    var parts = CsvParser.ParseLine(line);

                    if (parts.Length < 5)
                    {
                        skipped++;
                        continue;
                    }

                    try
                    {
                        var transaction = new Transaction
                        {
                            Date = DateTime.Parse(parts[0], CultureInfo.InvariantCulture),
                            Description = parts[1].Trim('"'),
                            Category = parts[2],
                            Amount = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                            Type = Enum.Parse<TransactionType>(parts[4], ignoreCase: true)
                        };

                        var duplicate = existing.Any(t =>
                            t.Date.Date == transaction.Date.Date &&
                            t.Description.Equals(transaction.Description, StringComparison.OrdinalIgnoreCase) &&
                            t.Amount == transaction.Amount);

                        if (duplicate)
                        {
                            skipped++;
                            continue;
                        }

                        await _dataService.AddTransactionAsync(transaction);
                        imported++;
                    }
                    catch
                    {
                        skipped++;
                    }
                }

                return Result.Success($"Imported {imported}, Skipped {skipped}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"CSV import failed: {ex.Message}");
            }
        }

        public async Task<Result> ImportFromJsonAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return Result.Failure("JSON file not found.");

                var json = await File.ReadAllTextAsync(filePath);

                var data = JsonSerializer.Deserialize<ExportPayload>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (data == null)
                    return Result.Failure("Invalid JSON structure.");

                int importedTx = 0;
                int importedBudgets = 0;

                var existingTx = (await _dataService.GetAllTransactionsAsync()).ToList();
                var existingBudgets = (await _dataService.GetAllBudgetsAsync()).ToList();

                foreach (var t in data.Transactions)
                {
                    var dup = existingTx.Any(e =>
                        e.Date.Date == t.Date.Date &&
                        e.Description.Equals(t.Description, StringComparison.OrdinalIgnoreCase) &&
                        e.Amount == t.Amount
                    );

                    if (!dup)
                    {
                        await _dataService.AddTransactionAsync(t);
                        importedTx++;
                    }
                }

                foreach (var b in data.Budgets)
                {
                    var dup = existingBudgets.Any(e =>
                        e.Name == b.Name && e.Category == b.Category
                    );

                    if (!dup)
                    {
                        await _dataService.AddBudgetAsync(b);
                        importedBudgets++;
                    }
                }

                return Result.Success($"Imported {importedTx} transactions, {importedBudgets} budgets");
            }
            catch (Exception ex)
            {
                return Result.Failure($"JSON import failed: {ex.Message}");
            }
        }
    }

    // ------------------------------------------------------------
    // SUPPORTING MODELS / UTILITIES
    // ------------------------------------------------------------

    public class ExportPayload
    {
        public DateTime ExportDate { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
        public List<Budget> Budgets { get; set; } = new();
        public User? User { get; set; }
    }
}

