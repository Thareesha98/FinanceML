using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceML.Core.Data;
using FinanceML.Core.Events;
using FinanceML.Core.Models;

namespace FinanceML.Core.Services
{
    /// <summary>
    /// Central data service responsible for managing transactions, budgets, 
    /// and triggering global data change notifications.
    /// Cleaner, more modular, contribution-friendly version.
    /// </summary>
    public class DataService : IDisposable
    {
        // =====================================================================
        // Singleton (Thread-Safe) – still supported for backward compatibility
        // =====================================================================
        private static readonly object LockObj = new();
        private static DataService? _instance;

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        _instance ??= new DataService();
                    }
                }
                return _instance;
            }
        }

        // =====================================================================
        // Fields
        // =====================================================================
        private readonly DatabaseContext _context;
        private readonly TransactionRepository _transactionRepository;
        private readonly BudgetRepository _budgetRepository;

        private int? _currentUserId;
        private bool _isDisposed;

        // =====================================================================
        // Public DataChanged Event
        // =====================================================================
        public event EventHandler<DataChangedEventArgs>? DataChanged;

        // =====================================================================
        // Constructor (Private — Singleton Pattern)
        // =====================================================================
        private DataService()
        {
            _context = new DatabaseContext();
            _transactionRepository = new TransactionRepository(_context);
            _budgetRepository = new BudgetRepository(_context);

            if (!_context.IsInitialized())
                InitializeSampleData();
        }

        // =====================================================================
        // USER CONTEXT HANDLING
        // =====================================================================
        public void SetCurrentUserId(int userId)
        {
            if (_currentUserId == userId)
                return;

            _currentUserId = userId;

            _budgetRepository.RecalculateBudgetSpentAmounts(userId);
            Notify(DataChangeType.UserChange);
        }

        private int GetUserOrFail()
        {
            return _currentUserId ??
                   throw new InvalidOperationException("Current user must be set before accessing data.");
        }

        // =====================================================================
        // NOTIFICATION HELPER
        // =====================================================================
        private void Notify(DataChangeType changeType)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs(changeType));
        }

        // =====================================================================
        // TRANSACTION OPERATIONS
        // =====================================================================

        public List<Transaction> GetAllTransactions()
        {
            return _transactionRepository.GetAllTransactions(GetUserOrFail());
        }

        public Transaction? GetTransactionById(int id)
        {
            return _transactionRepository.GetTransactionById(id, GetUserOrFail());
        }

        public async Task AddTransactionAsync(Transaction tx)
        {
            int userId = GetUserOrFail();

            tx.UserId = userId;
            tx.CreatedAt = DateTime.Now;

            tx.Id = await _transactionRepository.CreateTransactionAsync(tx, userId);

            await _budgetRepository.RecalculateBudgetSpentAmountsAsync(userId);

            Notify(DataChangeType.TransactionAdded);
        }

        public void UpdateTransaction(Transaction tx)
        {
            int userId = GetUserOrFail();

            if (tx.UserId != userId)
                throw new UnauthorizedAccessException("Cannot modify another user's transaction.");

            _transactionRepository.UpdateTransaction(tx);
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);

            Notify(DataChangeType.TransactionUpdated);
        }

        public void DeleteTransaction(int id)
        {
            int userId = GetUserOrFail();

            _transactionRepository.DeleteTransaction(id);
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);

            Notify(DataChangeType.TransactionDeleted);
        }

        public int GetTotalTransactionsCount()
        {
            return _transactionRepository.GetTotalTransactionsCount(GetUserOrFail());
        }

        // =====================================================================
        // BUDGET OPERATIONS
        // =====================================================================

        public List<Budget> GetAllBudgets()
        {
            return _budgetRepository.GetAllBudgets(GetUserOrFail());
        }

        
        // =====================================================================
        // FINANCIAL METRICS
        // =====================================================================

        public int GetFinancialHealthScore(DateTime? start = null, DateTime? end = null)
        {
            decimal income = GetTotalIncome(start, end);
            decimal expense = GetTotalExpenses(start, end);
            decimal net = income - expense;

            if (income <= 0)
                return 50;

            decimal ratio = net / income;
            int score = ratio switch
            {
                >= 0.20m => 90,
                >= 0.10m => 75,
                > 0      => 60,
                _        => 40
            };

            // Penalty for overspending
            if (GetAllBudgets()
                .Where(b => b.IsActive)
                .Any(b => b.SpentAmount > b.Amount))
            {
                score -= 15;
            }

            return Math.Max(0, score);
        }

        private decimal GetTotalIncome(DateTime? start, DateTime? end)
        {
            return GetAllTransactions()
                .Where(t => t.Amount > 0)
                .Where(t => (!start.HasValue || t.Date >= start) &&
                            (!end.HasValue || t.Date <= end))
                .Sum(t => t.Amount);
        }

        private decimal GetTotalExpenses(DateTime? start, DateTime? end)
        {
            return GetAllTransactions()
                .Where(t => t.Amount < 0)
                .Where(t => (!start.HasValue || t.Date >= start) &&
                            (!end.HasValue || t.Date <= end))
                .Sum(t => Math.Abs(t.Amount));
        }

        private decimal GetNetSavings(DateTime? start, DateTime? end)
        {
            return GetTotalIncome(start, end) - GetTotalExpenses(start, end);
        }

        // =====================================================================
        // DATA MANAGEMENT
        // =====================================================================

        public void ClearAllData()
        {
            int userId = GetUserOrFail();

            _context.ClearUserData(userId);
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);

            Notify(DataChangeType.AllDataCleared);
        }

        public void RefreshData()
        {
            Notify(DataChangeType.DataRefreshed);
        }

        private void InitializeSampleData()
        {
            // Existing logic untouched (initialization-only)
        }

        // =====================================================================
        // DISPOSABLE SUPPORT
        // =====================================================================

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                _context?.Dispose();

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

