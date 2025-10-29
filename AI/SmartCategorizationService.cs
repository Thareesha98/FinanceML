using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FinanceML.Core.Services;
using FinanceML.Core.Models;

namespace FinanceML.AI
{
    public class SmartCategorizationService
    {
        private static SmartCategorizationService? _instance;
        private readonly DataService _dataService;

        public static SmartCategorizationService Instance => _instance ??= new SmartCategorizationService();

        private SmartCategorizationService()
        {
            _dataService = DataService.Instance;
        }

        // Predefined category patterns
        private readonly Dictionary<string, List<string>> _categoryPatterns = new()
        {
            ["Food & Dining"] = new() { "restaurant", "cafe", "pizza", "burger", "food", "dining", "kitchen", "meal", "lunch", "dinner", "breakfast", "snack", "grocery", "supermarket", "market" },
            ["Transportation"] = new() { "uber", "taxi", "bus", "train", "metro", "fuel", "gas", "petrol", "parking", "toll", "transport", "travel", "flight", "airline" },
            ["Shopping"] = new() { "amazon", "flipkart", "mall", "store", "shop", "retail", "clothing", "fashion", "electronics", "gadget", "purchase" },
            ["Entertainment"] = new() { "movie", "cinema", "netflix", "spotify", "game", "entertainment", "concert", "show", "theater", "music", "streaming" },
            ["Utilities"] = new() { "electricity", "water", "gas", "internet", "phone", "mobile", "broadband", "utility", "bill", "recharge" },
            ["Healthcare"] = new() { "hospital", "doctor", "medical", "pharmacy", "medicine", "health", "clinic", "dental", "insurance" },
            ["Education"] = new() { "school", "college", "university", "course", "book", "education", "tuition", "fee", "learning" },
            ["Fitness"] = new() { "gym", "fitness", "yoga", "sports", "exercise", "workout", "health club" },
            ["Personal Care"] = new() { "salon", "spa", "beauty", "cosmetic", "haircut", "grooming", "personal care" },
            ["Home & Garden"] = new() { "furniture", "home", "garden", "repair", "maintenance", "cleaning", "decoration" },
            ["Investment"] = new() { "mutual fund", "stock", "share", "investment", "sip", "fd", "deposit", "portfolio" },
            ["Insurance"] = new() { "insurance", "premium", "policy", "life insurance", "health insurance" },
            ["Gifts & Donations"] = new() { "gift", "donation", "charity", "present", "contribution" },
            ["Business"] = new() { "office", "business", "professional", "meeting", "conference", "supplies" },
            ["Other"] = new() { "miscellaneous", "other", "unknown", "cash", "withdrawal", "transfer" }
        };

        public string SuggestCategory(string description, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "Other";

            var cleanDescription = description.ToLower().Trim();

            // First, try to match with historical data
            var historicalCategory = GetHistoricalCategory(cleanDescription);
            if (!string.IsNullOrEmpty(historicalCategory))
                return historicalCategory;

            // Then try pattern matching
            var patternCategory = GetPatternBasedCategory(cleanDescription);
            if (!string.IsNullOrEmpty(patternCategory))
                return patternCategory;

            // Amount-based categorization for common patterns
            var amountCategory = GetAmountBasedCategory(cleanDescription, amount);
            if (!string.IsNullOrEmpty(amountCategory))
                return amountCategory;

            return "Other";
        }

        private string GetHistoricalCategory(string description)
        {
            var transactions = _dataService.GetAllTransactions();
            
            // Look for exact matches first
            var exactMatch = transactions.FirstOrDefault(t => 
                t.Description.ToLower().Trim() == description);
            if (exactMatch != null)
                return exactMatch.Category;

            // Look for partial matches
            var partialMatches = transactions.Where(t => 
                t.Description.ToLower().Contains(description) || 
                description.Contains(t.Description.ToLower()))
                .GroupBy(t => t.Category)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            return partialMatches?.Key ?? string.Empty;
        }

        private string GetPatternBasedCategory(string description)
        {
            var scores = new Dictionary<string, int>();

            foreach (var category in _categoryPatterns)
            {
                var score = 0;
                foreach (var pattern in category.Value)
                {
                    if (description.Contains(pattern))
                    {
                        // Exact word match gets higher score
                        if (Regex.IsMatch(description, $@"\b{Regex.Escape(pattern)}\b"))
                            score += 3;
                        else
                            score += 1;
                    }
                }
                
                if (score > 0)
                    scores[category.Key] = score;
            }

            return scores.Any() ? scores.OrderByDescending(x => x.Value).First().Key : string.Empty;
        }

        private string GetAmountBasedCategory(string description, decimal amount)
        {
            var absAmount = Math.Abs(amount);

            // Small amounts often indicate food/snacks
            if (absAmount <= 100 && (description.Contains("pay") || description.Contains("quick")))
                return "Food & Dining";

            // Large amounts might be rent, investment, or major purchases
            if (absAmount >= 10000)
            {
                if (description.Contains("rent") || description.Contains("emi"))
                    return "Housing";
                if (description.Contains("invest") || description.Contains("fund"))
                    return "Investment";
                return "Other";
            }

            // Medium amounts for utilities
            if (absAmount >= 500 && absAmount <= 5000)
            {
                if (description.Contains("bill") || description.Contains("recharge"))
                    return "Utilities";
            }

            return string.Empty;
        }

        public List<string> GetCategorySuggestions(string partialDescription)
        {
            if (string.IsNullOrWhiteSpace(partialDescription))
                return _categoryPatterns.Keys.Take(5).ToList();

            var suggestions = new List<string>();
            var description = partialDescription.ToLower();

            // Get categories that match the description
            var matchingCategories = _categoryPatterns
                .Where(kvp => kvp.Value.Any(pattern => pattern.Contains(description) || description.Contains(pattern)))
                .Select(kvp => kvp.Key)
                .ToList();

            suggestions.AddRange(matchingCategories);

            // Add historical categories
            var transactions = _dataService.GetAllTransactions();
            var historicalCategories = transactions
                .Where(t => t.Description.ToLower().Contains(description))
                .Select(t => t.Category)
                .Distinct()
                .Where(c => !suggestions.Contains(c))
                .Take(3);

            suggestions.AddRange(historicalCategories);

            // Fill with most common categories if needed
            if (suggestions.Count < 5)
            {
                var commonCategories = _categoryPatterns.Keys
                    .Where(c => !suggestions.Contains(c))
                    .Take(5 - suggestions.Count);
                suggestions.AddRange(commonCategories);
            }

            return suggestions.Take(5).ToList();
        }

        public void LearnFromUserInput(string description, string userSelectedCategory)
        {
            // This method can be used to improve categorization based on user corrections
            // For now, it's a placeholder for future machine learning implementation
            
            // In a full implementation, this would:
            // 1. Store the user correction
            // 2. Update pattern weights
            // 3. Improve future suggestions
        }

        public Dictionary<string, int> GetCategoryUsageStats()
        {
            var transactions = _dataService.GetAllTransactions();
            return transactions
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public List<string> GetAllCategories()
        {
            var predefinedCategories = _categoryPatterns.Keys.ToList();
            var userCategories = _dataService.GetAllTransactions()
                .Select(t => t.Category)
                .Distinct()
                .Where(c => !predefinedCategories.Contains(c))
                .ToList();

            var allCategories = predefinedCategories.Concat(userCategories).Distinct().ToList();
            return allCategories.OrderBy(c => c).ToList();
        }
    }
}
