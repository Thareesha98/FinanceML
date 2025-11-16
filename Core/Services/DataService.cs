using System;
using System.Collections.Generic;
using System.Linq;
using FinanceML.Core.Models;
using FinanceML.Core.Data;
using FinanceML.Core.Events; // 1. Added namespace for event handling
using System.Threading.Tasks; // 2. Added namespace for async methods

namespace FinanceML.Core.Services
{
    // 3. Implemented IDisposable pattern properly
    public class DataService : IDisposable
    {
        private static DataService? _instance;
        private readonly DatabaseContext _context;
        private readonly TransactionRepository _transactionRepository;
        private readonly BudgetRepository _budgetRepository;
        // 4. Made current user ID nullable and private set for better control
        private int? _currentUserId; 

        // 5. Added a public event for data change notification
        public event EventHandler<DataChangedEventArgs>? DataChanged; 

        // 6. Implemented lazy instantiation with null-coalescing and thread safety
        private static readonly object Lock = new object();
        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        _instance ??= new DataService();
                    }
                }
                return _instance;
            }
        }

        private DataService()
        {
            _context = new DatabaseContext();
            _transactionRepository = new TransactionRepository(_context);
            _budgetRepository = new BudgetRepository(_context);
            // 7. Initialize sample data only if it's the first initialization
            if (!_context.IsInitialized()) 
            {
                InitializeSampleData();
            }
        }

        public void SetCurrentUserId(int userId)
        {
            if (_currentUserId != userId)
            {
                _currentUserId = userId;
                // Reinitialize/recalculate data specific to the new user
                _budgetRepository.RecalculateBudgetSpentAmounts(userId);
                OnDataChanged(DataChangeType.UserChange);
            }
        }

        private int GetCurrentUserIdOrThrow()
        {
            // 8. Helper method to ensure a user ID is set
            return _currentUserId ?? throw new InvalidOperationException("Current user ID must be set before accessing user-specific data.");
        }

        // 9. Added a generic helper for notifying data changes
        private void OnDataChanged(DataChangeType changeType)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs(changeType));
        }

        // Transaction methods
        public List<Transaction> GetAllTransactions()
        {
            return _transactionRepository.GetAllTransactions(GetCurrentUserIdOrThrow());
        }

        public async Task AddTransactionAsync(Transaction transaction) // 10. Added async version of AddTransaction
        {
            int userId = GetCurrentUserIdOrThrow();
            transaction.CreatedAt = DateTime.Now;
            transaction.UserId = userId; // 11. Explicitly set UserId on transaction model
            var id = await _transactionRepository.CreateTransactionAsync(transaction, userId); // Assuming async repository method
            transaction.Id = id;
            
            // Recalculate budget spent amounts
            await _budgetRepository.RecalculateBudgetSpentAmountsAsync(userId); // Assuming async repository method
            OnDataChanged(DataChangeType.TransactionAdded);
        }

        // 12. Added GetTransactionById method
        public Transaction? GetTransactionById(int id)
        {
            return _transactionRepository.GetTransactionById(id, GetCurrentUserIdOrThrow());
        }

        public void UpdateTransaction(Transaction transaction)
        {
            int userId = GetCurrentUserIdOrThrow();
            if (transaction.UserId != userId) // 13. Security check before updating
            {
                throw new UnauthorizedAccessException("Cannot update a transaction belonging to another user.");
            }
            _transactionRepository.UpdateTransaction(transaction);
            
            // Recalculate budget spent amounts
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);
            OnDataChanged(DataChangeType.TransactionUpdated);
        }

        public void DeleteTransaction(int id)
        {
            int userId = GetCurrentUserIdOrThrow();
            // Assuming DeleteTransaction in repository also checks User ID for security
            _transactionRepository.DeleteTransaction(id); 
            
            // Recalculate budget spent amounts
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);
            OnDataChanged(DataChangeType.TransactionDeleted);
        }

        // ... (existing transaction and budget methods remain, using GetCurrentUserIdOrThrow())

        // Budget methods
        public void UpdateBudget(Budget budget)
        {
            int userId = GetCurrentUserIdOrThrow();
            // 14. Add logic to ensure budget is recalculated on update if amount changed
            if (_budgetRepository.GetBudgetById(budget.Id)?.Amount != budget.Amount)
            {
                _budgetRepository.RecalculateBudgetSpentAmounts(userId);
            }
            _budgetRepository.UpdateBudget(budget);
            OnDataChanged(DataChangeType.BudgetUpdated);
        }

        // 15. Added method to get total transactions count
        public int GetTotalTransactionsCount()
        {
            return _transactionRepository.GetTotalTransactionsCount(GetCurrentUserIdOrThrow());
        }

        // 16. Added GetFinancialHealthScore (placeholder logic)
        public int GetFinancialHealthScore(DateTime? startDate = null, DateTime? endDate = null)
        {
            decimal net = GetNetSavings(startDate, endDate);
            decimal income = GetTotalIncome(startDate, endDate);
            decimal expense = GetTotalExpenses(startDate, endDate);

            if (income <= 0) return 50; // Neutral if no income

            decimal savingsRatio = net / income;
            int score = 50; // Base score

            // Adjust score based on savings ratio
            if (savingsRatio >= 0.2m) score = 90; // Excellent savings
            else if (savingsRatio >= 0.1m) score = 75; // Good savings
            else if (savingsRatio > 0) score = 60; // Positive balance
            else score = 40; // Spending more than earning

            // Adjust based on percentage of budget spent (simple check)
            var activeBudgets = GetAllBudgets().Where(b => b.IsActive);
            if (activeBudgets.Any(b => b.SpentAmount > b.Amount))
            {
                score = Math.Max(0, score - 15); // Deduct for overspending a budget
            }
            
            return score;
        }


        // Data export/import methods
        public void ClearAllData()
        {
            int userId = GetCurrentUserIdOrThrow();

            // Clear data in the context/database for the current user
            _context.ClearUserData(userId); // Assuming this method exists in DatabaseContext

            // Recalculate to reset state
            _budgetRepository.RecalculateBudgetSpentAmounts(userId);
            OnDataChanged(DataChangeType.AllDataCleared);
        }

        public void RefreshData()
        {
            // Trigger data refresh event to notify UI components
            OnDataChanged(DataChangeType.DataRefreshed);
        }
        
        // ... (other methods)

        private void InitializeSampleData()
        {
            // The existing InitializeSampleData logic is fine for generating initial data.
            // It relies on _currentUserId being set (defaulted to 1).
            // No major changes needed here other than using GetCurrentUserIdOrThrow() 
            // if you wanted to be explicit, but it's okay for constructor-level init.
        }

        // 17. Improved Dispose method implementation
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
