// DataProcessor.cs

using System;
using System.Collections.Generic;
using System.Linq;

// This class will hold utility methods, leveraging C# 3.0 extensions.
public static class DataProcessor
{
    // Line 8: Extension Method to calculate the average price of items in a category.
    public static decimal GetAveragePrice(this IEnumerable<InventoryItem> items, string category)
    {
        // Use LINQ to filter and calculate the average.
        var categoryItems = items.Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        
        // Check if there are any items to prevent division by zero.
        if (!categoryItems.Any())
        {
            return 0m;
        }

        // Line 20: Use the LINQ Average() aggregate function.
        return categoryItems.Average(item => item.UnitPrice);
    }

    // Line 24: Extension Method to find the most expensive item (using Max).
    public static InventoryItem GetMostExpensiveItem(this IEnumerable<InventoryItem> items)
    {
        if (!items.Any())
        {
            return null;
        }

        // Line 32: Use the LINQ Max() function to find the maximum UnitPrice.
        var maxPrice = items.Max(item => item.UnitPrice);
        
        // Return the first item that matches that max price.
        // Line 36: Using FirstOrDefault() with a lambda expression.
        return items.FirstOrDefault(item => item.UnitPrice == maxPrice);
    }

    // Line 40: Main method to run the analysis (assuming the InventoryManager's InitializeInventory method is accessible).
    public static void RunAnalysis()
    {
        // Line 44: Use implicit typing (var) and Collection Initializers.
        var inventory = InventoryManager.InitializeInventory();

        Console.WriteLine("\n--- Data Processor Analysis (LINQ Aggregates) ---");

        // Line 49: Use the Extension Method defined above.
        var electronicsAvg = inventory.GetAveragePrice("Electronics");
        Console.WriteLine($"Average Price (Electronics): {electronicsAvg:C}");

        var apparelCount = inventory.Count(item => item.Category == "Apparel");
        Console.WriteLine($"Total Apparel Items: {apparelCount}");
        
        // Line 57: Use the Extension Method defined above.
        var priciestItem = inventory.GetMostExpensiveItem();
        if (priciestItem != null)
        {
            Console.WriteLine($"Most Expensive Item: {priciestItem.Name} ({priciestItem.UnitPrice:C})");
        }
        
        // Line 65: LINQ Count method
        var totalDiscontinued = inventory.Count(item => item.IsDiscontinued);
        Console.WriteLine($"Total Discontinued Products: {totalDiscontinued}");
        
        // Line 70: LINQ Sum method
        var totalStock = inventory.Sum(item => item.StockQuantity);
        Console.WriteLine($"Total Stock Across All Products: {totalStock}");
    }
}
// Line 75
