using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SimpleFinanceApp
{
    // Transaction types
    public enum TransactionType { Income, Expense, Transfer }

    // Transaction model
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; } = "General";
        public string Description { get; set; } = "";
        public string FromAccount { get; set; } = "";
        public string ToAccount { get; set; } = "";

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd} | {Type} | {Amount:C} | {Category} | {Description}";
        }

        public string ToCsv()
        {
            return $"{Id},{Date:O},{Amount},{Type},{Category},{EscapeCsv(Description)},{FromAccount},{ToAccount}";
        }

        private static string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            return s;
        }
    }

    // Account model
    public class Account
    {
        public string Name { get; set; } = "";
        public decimal Balance { get; set; } = 0m;
        public string Currency { get; set; } = "LKR";

        public override string ToString()
        {
            return $"{Name} - {Balance:C} {Currency}";
        }
    }

    // Budget model for categories per month
    public class Budget
    {
        public string Category { get; set; } = "General";
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Limit { get; set; }

        public bool Matches(DateTime d) => Year == d.Year && Month == d.Month;
    }

    // In-memory datastore
    public class FinanceStore
    {
        public List<Account> Accounts { get; } = new();
        public List<Transaction> Transactions { get; } = new();
        public List<Budget> Budgets { get; } = new();

        public FinanceStore()
        {
            Seed();
        }

        private void Seed()
        {
            // Seed sample accounts
            Accounts.Add(new Account { Name = "Cash", Balance = 50000m });
            Accounts.Add(new Account { Name = "Bank-Current", Balance = 150000m });
            Accounts.Add(new Account { Name = "Card-Visa", Balance = -20000m });

            // Seed sample transactions for current month
            var now = DateTime.Now;
            Transactions.Add(new Transaction
            {
                Date = now.AddDays(-10),
                Amount = 45000m,
                Type = TransactionType.Income,
                Category = "Salary",
                Description = "October salary",
                ToAccount = "Bank-Current"
            });

            Transactions.Add(new Transaction
            {
                Date = now.AddDays(-7),
                Amount = 2000m,
                Type = TransactionType.Expense,
                Category = "Groceries",
                Description = "Weekly groceries",
                FromAccount = "Cash"
            });

            Transactions.Add(new Transaction
            {
                Date = now.AddDays(-3),
                Amount = 15000m,
                Type = TransactionType.Expense,
                Category = "Rent",
                Description = "Monthly rent",
                FromAccount = "Bank-Current"
            });

            // Seed budgets
            Budgets.Add(new Budget { Category = "Groceries", Year = now.Year, Month = now.Month, Limit = 10000m });
            Budgets.Add(new Budget { Category = "Entertainment", Year = now.Year, Month = now.Month, Limit = 5000m });
        }

        public void AddTransaction(Transaction t)
        {
            Transactions.Add(t);

            switch (t.Type)
            {
                case TransactionType.Income:
                    if (!string.IsNullOrWhiteSpace(t.ToAccount))
                    {
                        var acct = Accounts.FirstOrDefault(a => a.Name == t.ToAccount);
                        if (acct != null) acct.Balance += t.Amount;
                    }
                    break;

                case TransactionType.Expense:
                    if (!string.IsNullOrWhiteSpace(t.FromAccount))
                    {
                        var acct = Accounts.FirstOrDefault(a => a.Name == t.FromAccount);
                        if (acct != null) acct.Balance -= t.Amount;
                    }
                    break;

                case TransactionType.Transfer:
                    if (!string.IsNullOrWhiteSpace(t.FromAccount))
                    {
                        var from = Accounts.FirstOrDefault(a => a.Name == t.FromAccount);
                        if (from != null) from.Balance -= t.Amount;
                    }
                    if (!string.IsNullOrWhiteSpace(t.ToAccount))
                    {
                        var to = Accounts.FirstOrDefault(a => a.Name == t.ToAccount);
                        if (to != null) to.Balance += t.Amount;
                    }
                    break;
            }
        }

        public IEnumerable<Transaction> GetTransactionsForMonth(int year, int month)
        {
            return Transactions.Where(t => t.Date.Year == year && t.Date.Month == month).OrderByDescending(t => t.Date);
        }

        public decimal GetTotalByType(int year, int month, TransactionType type)
        {
            return Transactions.Where(t => t.Date.Year == year && t.Date.Month == month && t.Type == type)
                                .Sum(t => t.Amount);
        }

        public decimal GetSpentInCategory(int year, int month, string category)
        {
            return Transactions.Where(t => t.Date.Year == year && t.Date.Month == month && t.Type == TransactionType.Expense && t.Category == category)
                               .Sum(t => t.Amount);
        }
    }

    // Simple reporting utilities
    public static class Reports
    {
        public static void PrintMonthlySummary(FinanceStore store, int year, int month)
        {
            Console.WriteLine($"--- Summary for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year} ---");
            var income = store.GetTotalByType(year, month, TransactionType.Income);
            var expense = store.GetTotalByType(year, month, TransactionType.Expense);
            var net = income - expense;
            Console.WriteLine($"Income : {income:C}");
            Console.WriteLine($"Expense: {expense:C}");
            Console.WriteLine($"Net    : {net:C}");
            Console.WriteLine();
        }

        public static void PrintCategoryBudgetStatus(FinanceStore store, int year, int month)
        {
            Console.WriteLine("--- Budget Status ---");
            foreach (var b in store.Budgets.Where(b => b.Year == year && b.Month == month))
            {
                var spent = store.GetSpentInCategory(year, month, b.Category);
                Console.WriteLine($"{b.Category} - Spent: {spent:C} / Limit: {b.Limit:C}  ({(b.Limit == 0 ? 0 : (spent / b.Limit * 100)):0.##}%)");
            }
            Console.WriteLine();
        }

        public static void PrintAccounts(FinanceStore store)
        {
            Console.WriteLine("--- Accounts ---");
            foreach (var a in store.Accounts)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine();
        }

        public static void ExportTransactionsCsv(FinanceStore store, string path)
        {
            using var sw = new StreamWriter(path);
            sw.WriteLine("Id,Date,Amount,Type,Category,Description,FromAccount,ToAccount");
            foreach (var t in store.Transactions.OrderBy(t => t.Date))
            {
                sw.WriteLine(t.ToCsv());
            }
        }
    }

    // Console UI
    public static class ConsoleUi
    {
        public static void ShowMenu()
        {
            Console.WriteLine("=== Simple Financial Management ===");
            Console.WriteLine("1) View accounts");
            Console.WriteLine("2) View transactions (this month)");
            Console.WriteLine("3) Add transaction");
            Console.WriteLine("4) Monthly summary & budgets");
            Console.WriteLine("5) Export transactions (CSV)");
            Console.WriteLine("0) Exit");
            Console.Write("Choose an option: ");
        }

        public static Transaction ReadTransactionFromUser(FinanceStore store)
        {
            Console.Write("Type (income/expense/transfer): ");
            var typeStr = Console.ReadLine()?.Trim().ToLower();
            TransactionType type = typeStr switch
            {
                "income" => TransactionType.Income,
                "transfer" => TransactionType.Transfer,
                _ => TransactionType.Expense
            };

            Console.Write("Amount: ");
            var amtText = Console.ReadLine() ?? "0";
            decimal amount = decimal.TryParse(amtText, out var a) ? a : 0m;

            Console.Write("Date (yyyy-MM-dd) [leave empty for today]: ");
            var dateText = Console.ReadLine();
            DateTime date = DateTime.TryParse(dateText, out var d) ? d : DateTime.Now;

            Console.Write("Category: ");
            var category = Console.ReadLine() ?? "General";

            Console.Write("Description: ");
            var desc = Console.ReadLine() ?? "";

            string from = "", to = "";
            if (type == TransactionType.Expense)
            {
                Console.Write("From account: ");
                from = Console.ReadLine() ?? "";
            }
            else if (type == TransactionType.Income)
            {
                Console.Write("To account: ");
                to = Console.ReadLine() ?? "";
            }
            else // transfer
            {
                Console.Write("From account: ");
                from = Console.ReadLine() ?? "";
                Console.Write("To account: ");
                to = Console.ReadLine() ?? "";
            }

            return new Transaction
            {
                Date = date,
                Amount = amount,
                Type = type,
                Category = category,
                Description = desc,
                FromAccount = from,
                ToAccount = to
            };
        }
    }

    // Program entry
    class Program
    {
        static void Main(string[] args)
        {
            var store = new FinanceStore();
            bool running = true;

            while (running)
            {
                Console.Clear();
                ConsoleUi.ShowMenu();
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Reports.PrintAccounts(store);
                        Pause();
                        break;

                    case "2":
                        Console.Clear();
                        var now = DateTime.Now;
                        var txs = store.GetTransactionsForMonth(now.Year, now.Month);
                        Console.WriteLine("--- Transactions (this month) ---");
                        foreach (var t in txs) Console.WriteLine(t);
                        Console.WriteLine();
                        Pause();
                        break;

                    case "3":
                        Console.Clear();
                        var newTx = ConsoleUi.ReadTransactionFromUser(store);
                        store.AddTransaction(newTx);
                        Console.WriteLine("Transaction added.");
                        Pause();
                        break;

                    case "4":
                        Console.Clear();
                        var y = DateTime.Now.Year;
                        var m = DateTime.Now.Month;
                        Reports.PrintMonthlySummary(store, y, m);
                        Reports.PrintCategoryBudgetStatus(store, y, m);
                        Pause();
                        break;

                    case "5":
                        Console.Clear();
                        Console.Write("Enter export file path (e.g. transactions.csv): ");
                        var path = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(path)) path = "transactions.csv";
                        try
                        {
                            Reports.ExportTransactionsCsv(store, path);
                            Console.WriteLine($"Exported to {Path.GetFullPath(path)}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Export failed: " + ex.Message);
                        }
                        Pause();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Unknown option.");
                        Pause();
                        break;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}

