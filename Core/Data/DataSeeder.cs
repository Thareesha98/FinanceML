using System;
using System.Collections.Generic;
using FinanceML.Core.Models;

namespace FinanceML.Core.Data
{
    public static class DataSeeder
    {
        public static void SeedUserData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            // Check if user already has data
            var existingTransactions = transactionRepo.GetAllTransactions(userId);
            if (existingTransactions.Count > 0)
                return; // User already has data

            // Generate different financial profiles based on userId
            switch (userId % 5)
            {
                case 1: // High earner, tech professional
                    SeedTechProfessionalData(userId, transactionRepo, budgetRepo);
                    break;
                case 2: // Student with part-time job
                    SeedStudentData(userId, transactionRepo, budgetRepo);
                    break;
                case 3: // Family with children
                    SeedFamilyData(userId, transactionRepo, budgetRepo);
                    break;
                case 4: // Freelancer with irregular income
                    SeedFreelancerData(userId, transactionRepo, budgetRepo);
                    break;
                default: // Average working professional
                    SeedAverageWorkerData(userId, transactionRepo, budgetRepo);
                    break;
            }

            // Recalculate budget spent amounts
            budgetRepo.RecalculateBudgetSpentAmounts(userId);
        }

        private static void SeedTechProfessionalData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            var transactions = new List<Transaction>
            {
                // High salary income
                new Transaction { Date = DateTime.Now.AddDays(-30), Description = "Software Engineer Salary", Category = "Salary", Amount = 8500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-30) },
                new Transaction { Date = DateTime.Now.AddDays(-60), Description = "Software Engineer Salary", Category = "Salary", Amount = 8500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-60) },
                new Transaction { Date = DateTime.Now.AddDays(-90), Description = "Software Engineer Salary", Category = "Salary", Amount = 8500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-90) },
                
                // Tech-related expenses
                new Transaction { Date = DateTime.Now.AddDays(-5), Description = "MacBook Pro", Category = "Technology", Amount = 2500m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-5) },
                new Transaction { Date = DateTime.Now.AddDays(-15), Description = "AWS Subscription", Category = "Technology", Amount = 150m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-15) },
                new Transaction { Date = DateTime.Now.AddDays(-20), Description = "GitHub Pro", Category = "Technology", Amount = 20m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-20) },
                new Transaction { Date = DateTime.Now.AddDays(-25), Description = "Udemy Courses", Category = "Education", Amount = 200m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-25) },
                
                // High-end lifestyle
                new Transaction { Date = DateTime.Now.AddDays(-3), Description = "Fine Dining Restaurant", Category = "Food & Dining", Amount = 180m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-3) },
                new Transaction { Date = DateTime.Now.AddDays(-7), Description = "Whole Foods Grocery", Category = "Food & Dining", Amount = 220m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-7) },
                new Transaction { Date = DateTime.Now.AddDays(-12), Description = "Tesla Supercharger", Category = "Transportation", Amount = 45m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-12) },
                new Transaction { Date = DateTime.Now.AddDays(-18), Description = "Premium Gym Membership", Category = "Health & Fitness", Amount = 120m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-18) },
                
                // Investment and savings
                new Transaction { Date = DateTime.Now.AddDays(-10), Description = "Stock Investment", Category = "Investments", Amount = 2000m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-10) },
                new Transaction { Date = DateTime.Now.AddDays(-30), Description = "401k Contribution", Category = "Savings", Amount = 1500m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-30) }
            };

            foreach (var transaction in transactions)
            {
                transactionRepo.CreateTransaction(transaction, userId);
            }

            var budgets = new List<Budget>
            {
                new Budget { Name = "Technology Budget", Category = "Technology", Amount = 1000m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Food & Dining Budget", Category = "Food & Dining", Amount = 800m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Investment Budget", Category = "Investments", Amount = 3000m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true }
            };

            foreach (var budget in budgets)
            {
                budgetRepo.CreateBudget(budget, userId);
            }
        }

        private static void SeedStudentData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            var transactions = new List<Transaction>
            {
                // Part-time job income
                new Transaction { Date = DateTime.Now.AddDays(-15), Description = "Part-time Job - Retail", Category = "Part-time Work", Amount = 800m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-15) },
                new Transaction { Date = DateTime.Now.AddDays(-45), Description = "Part-time Job - Retail", Category = "Part-time Work", Amount = 750m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-45) },
                new Transaction { Date = DateTime.Now.AddDays(-75), Description = "Financial Aid", Category = "Financial Aid", Amount = 2500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-75) },
                
                // Student expenses
                new Transaction { Date = DateTime.Now.AddDays(-2), Description = "Textbooks", Category = "Education", Amount = 350m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-2) },
                new Transaction { Date = DateTime.Now.AddDays(-5), Description = "Campus Meal Plan", Category = "Food & Dining", Amount = 450m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-5) },
                new Transaction { Date = DateTime.Now.AddDays(-8), Description = "Bus Pass", Category = "Transportation", Amount = 65m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-8) },
                new Transaction { Date = DateTime.Now.AddDays(-12), Description = "Coffee Shop Study Session", Category = "Food & Dining", Amount = 15m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-12) },
                new Transaction { Date = DateTime.Now.AddDays(-18), Description = "Movie Night", Category = "Entertainment", Amount = 12m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-18) },
                new Transaction { Date = DateTime.Now.AddDays(-22), Description = "Laundromat", Category = "Personal Care", Amount = 8m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-22) },
                new Transaction { Date = DateTime.Now.AddDays(-28), Description = "Phone Bill", Category = "Bills & Utilities", Amount = 45m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-28) }
            };

            foreach (var transaction in transactions)
            {
                transactionRepo.CreateTransaction(transaction, userId);
            }

            var budgets = new List<Budget>
            {
                new Budget { Name = "Food Budget", Category = "Food & Dining", Amount = 300m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Transportation Budget", Category = "Transportation", Amount = 100m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Entertainment Budget", Category = "Entertainment", Amount = 50m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true }
            };

            foreach (var budget in budgets)
            {
                budgetRepo.CreateBudget(budget, userId);
            }
        }

        private static void SeedFamilyData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            var transactions = new List<Transaction>
            {
                // Family income
                new Transaction { Date = DateTime.Now.AddDays(-30), Description = "Primary Income", Category = "Salary", Amount = 6500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-30) },
                new Transaction { Date = DateTime.Now.AddDays(-30), Description = "Spouse Income", Category = "Salary", Amount = 4200m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-30) },
                
                // Family expenses
                new Transaction { Date = DateTime.Now.AddDays(-3), Description = "Grocery Shopping - Family", Category = "Food & Dining", Amount = 280m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-3) },
                new Transaction { Date = DateTime.Now.AddDays(-7), Description = "Daycare Payment", Category = "Childcare", Amount = 1200m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-7) },
                new Transaction { Date = DateTime.Now.AddDays(-10), Description = "Mortgage Payment", Category = "Housing", Amount = 2200m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-10) },
                new Transaction { Date = DateTime.Now.AddDays(-12), Description = "Kids Clothing", Category = "Shopping", Amount = 150m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-12) },
                new Transaction { Date = DateTime.Now.AddDays(-15), Description = "Family Car Insurance", Category = "Transportation", Amount = 180m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-15) },
                new Transaction { Date = DateTime.Now.AddDays(-18), Description = "Pediatrician Visit", Category = "Healthcare", Amount = 120m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-18) },
                new Transaction { Date = DateTime.Now.AddDays(-20), Description = "Family Fun Center", Category = "Entertainment", Amount = 85m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-20) },
                new Transaction { Date = DateTime.Now.AddDays(-25), Description = "School Supplies", Category = "Education", Amount = 95m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-25) }
            };

            foreach (var transaction in transactions)
            {
                transactionRepo.CreateTransaction(transaction, userId);
            }

            var budgets = new List<Budget>
            {
                new Budget { Name = "Family Food Budget", Category = "Food & Dining", Amount = 1000m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Childcare Budget", Category = "Childcare", Amount = 1500m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Housing Budget", Category = "Housing", Amount = 2500m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Family Entertainment", Category = "Entertainment", Amount = 300m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true }
            };

            foreach (var budget in budgets)
            {
                budgetRepo.CreateBudget(budget, userId);
            }
        }

        private static void SeedFreelancerData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            var transactions = new List<Transaction>
            {
                // Irregular freelance income
                new Transaction { Date = DateTime.Now.AddDays(-5), Description = "Web Design Project", Category = "Freelance", Amount = 2500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-5) },
                new Transaction { Date = DateTime.Now.AddDays(-20), Description = "Logo Design", Category = "Freelance", Amount = 800m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-20) },
                new Transaction { Date = DateTime.Now.AddDays(-35), Description = "Consulting Work", Category = "Freelance", Amount = 1500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-35) },
                new Transaction { Date = DateTime.Now.AddDays(-50), Description = "Mobile App Development", Category = "Freelance", Amount = 3200m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-50) },
                
                // Business expenses
                new Transaction { Date = DateTime.Now.AddDays(-8), Description = "Adobe Creative Suite", Category = "Business Expenses", Amount = 60m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-8) },
                new Transaction { Date = DateTime.Now.AddDays(-12), Description = "Co-working Space", Category = "Business Expenses", Amount = 200m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-12) },
                new Transaction { Date = DateTime.Now.AddDays(-15), Description = "Business Internet", Category = "Bills & Utilities", Amount = 85m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-15) },
                new Transaction { Date = DateTime.Now.AddDays(-22), Description = "Client Meeting - Coffee", Category = "Business Expenses", Amount = 25m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-22) },
                new Transaction { Date = DateTime.Now.AddDays(-28), Description = "Freelancer Tax Payment", Category = "Taxes", Amount = 800m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-28) },
                
                // Personal expenses
                new Transaction { Date = DateTime.Now.AddDays(-4), Description = "Grocery Shopping", Category = "Food & Dining", Amount = 120m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-4) },
                new Transaction { Date = DateTime.Now.AddDays(-18), Description = "Health Insurance", Category = "Healthcare", Amount = 350m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-18) }
            };

            foreach (var transaction in transactions)
            {
                transactionRepo.CreateTransaction(transaction, userId);
            }

            var budgets = new List<Budget>
            {
                new Budget { Name = "Business Expenses", Category = "Business Expenses", Amount = 500m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Tax Savings", Category = "Taxes", Amount = 1000m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Emergency Fund", Category = "Savings", Amount = 800m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true }
            };

            foreach (var budget in budgets)
            {
                budgetRepo.CreateBudget(budget, userId);
            }
        }

        private static void SeedAverageWorkerData(int userId, TransactionRepository transactionRepo, BudgetRepository budgetRepo)
        {
            var transactions = new List<Transaction>
            {
                // Regular salary
                new Transaction { Date = DateTime.Now.AddDays(-30), Description = "Monthly Salary", Category = "Salary", Amount = 4500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-30) },
                new Transaction { Date = DateTime.Now.AddDays(-60), Description = "Monthly Salary", Category = "Salary", Amount = 4500m, Type = TransactionType.Income, CreatedAt = DateTime.Now.AddDays(-60) },
                
                // Regular expenses
                new Transaction { Date = DateTime.Now.AddDays(-5), Description = "Rent Payment", Category = "Housing", Amount = 1200m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-5) },
                new Transaction { Date = DateTime.Now.AddDays(-8), Description = "Grocery Shopping", Category = "Food & Dining", Amount = 180m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-8) },
                new Transaction { Date = DateTime.Now.AddDays(-12), Description = "Electric Bill", Category = "Bills & Utilities", Amount = 95m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-12) },
                new Transaction { Date = DateTime.Now.AddDays(-15), Description = "Gas Station", Category = "Transportation", Amount = 55m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-15) },
                new Transaction { Date = DateTime.Now.AddDays(-18), Description = "Internet Bill", Category = "Bills & Utilities", Amount = 65m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-18) },
                new Transaction { Date = DateTime.Now.AddDays(-22), Description = "Lunch Out", Category = "Food & Dining", Amount = 25m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-22) },
                new Transaction { Date = DateTime.Now.AddDays(-25), Description = "Netflix Subscription", Category = "Entertainment", Amount = 15m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-25) },
                new Transaction { Date = DateTime.Now.AddDays(-28), Description = "Gym Membership", Category = "Health & Fitness", Amount = 45m, Type = TransactionType.Expense, CreatedAt = DateTime.Now.AddDays(-28) }
            };

            foreach (var transaction in transactions)
            {
                transactionRepo.CreateTransaction(transaction, userId);
            }

            var budgets = new List<Budget>
            {
                new Budget { Name = "Housing Budget", Category = "Housing", Amount = 1300m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Food Budget", Category = "Food & Dining", Amount = 400m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true },
                new Budget { Name = "Transportation Budget", Category = "Transportation", Amount = 200m, Period = BudgetPeriod.Monthly, StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), CreatedAt = DateTime.Now, IsActive = true }
            };

            foreach (var budget in budgets)
            {
                budgetRepo.CreateBudget(budget, userId);
            }
        }
    }
}
