// Line 1: Using necessary namespaces
using System;
using System.Collections.Generic;
using System.Linq; // Essential for LINQ

// Line 5: Define the core data model (simple C# 3.0 class)
public class InventoryItem
{
    // Line 9: Auto-implemented properties (introduced in C# 3.0)
    public int ItemID { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime LastRestockDate { get; set; }
    public bool IsDiscontinued { get; set; }

    // Line 18: Constructor (optional, but good practice)
    public InventoryItem(int id, string name, string category, int quantity, decimal price, DateTime restockDate, bool discontinued = false)
    {
        ItemID = id;
        Name = name;
        Category = category;
        StockQuantity = quantity;
        UnitPrice = price;
        LastRestockDate = restockDate;
        IsDiscontinued = discontinued;
    }

    // Line 29: Override ToString for easy printing
    public override string ToString()
    {
        return $"ID: {ItemID}, Name: {Name.PadRight(15)}, Cat: {Category.PadRight(10)}, Qty: {StockQuantity.ToString().PadLeft(4)}, Price: {UnitPrice:C}, Restock: {LastRestockDate.ToShortDateString()}, Disc: {IsDiscontinued}";
    }
}

// Line 37: Main Application Class
public class InventoryManager
{
    // Line 40: Main entry point
    public static void Main(string[] args)
    {
        // Line 43: Use var (implicit typing) for collection declaration (C# 3.0 feature)
        var inventory = InitializeInventory();

        Console.WriteLine("--- Full Inventory Listing ---");
        // Line 47: Use foreach with var (implicit typing)
        foreach (var item in inventory)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("\n--- 1. Low Stock Items (< 50) using Query Syntax ---");
        // Line 55: LINQ Query Syntax for filtering
        var lowStockItems = from item in inventory
                            where item.StockQuantity < 50 && item.IsDiscontinued == false
                            orderby item.StockQuantity ascending
                            select item;
        
        PrintItems(lowStockItems);

        Console.WriteLine("\n--- 2. High Value Products (> $25) using Method Syntax (Lambda) ---");
        // Line 65: LINQ Method Syntax with Lambda Expression (C# 3.0 feature)
        var highValueProducts = inventory
                                .Where(item => item.UnitPrice > 25.00m)
                                .OrderByDescending(item => item.UnitPrice)
                                .Select(item => new { item.Name, item.UnitPrice }); // Anonymous Type (C# 3.0 feature)
        
        // Line 72: Iterating over an Anonymous Type
        foreach (var product in highValueProducts)
        {
            Console.WriteLine($"Product: {product.Name.PadRight(15)}, Price: {product.UnitPrice:C}");
        }


        Console.WriteLine("\n--- 3. Grouped by Category and Total Value (Mixed Syntax) ---");
        // Line 79: Complex LINQ Query using grouping and aggregation
        var categorySummary = from item in inventory
                              group item by item.Category into categoryGroup
                              select new // Another Anonymous Type
                              {
                                  Category = categoryGroup.Key,
                                  TotalStock = categoryGroup.Sum(item => item.StockQuantity),
                                  // Line 87: Calculate total inventory value for the category
                                  TotalCategoryValue = categoryGroup.Sum(item => item.StockQuantity * item.UnitPrice)
                              };

        // Line 92: Print Category Summary
        foreach (var summary in categorySummary)
        {
            Console.WriteLine($"Category: {summary.Category.PadRight(10)} | Total Qty: {summary.TotalStock.ToString().PadLeft(5)} | Total Value: {summary.TotalCategoryValue:C}");
        }

        Console.WriteLine("\n--- 4. Items Restocked This Year (Using an Extension Method) ---");
        // Line 99: Using an external Extension Method (defined below)
        var recentlyRestocked = inventory.RestockedAfter(new DateTime(2025, 1, 1));

        PrintItems(recentlyRestocked);

        Console.WriteLine("\n--- 5. First 3 Items in 'Electronics' Category ---");
        // Line 106: Using LINQ for projection and limiting results
        var top3Electronics = inventory
                                .Where(item => item.Category == "Electronics")
                                .Take(3)
                                .Select(item => item.Name); // Projecting to just the Name string

        // Line 112: Print projected strings
        foreach (var name in top3Electronics)
        {
            Console.WriteLine($"- {name}");
        }
    }

    // Line 119: Helper method to populate initial data
    private static List<InventoryItem> InitializeInventory()
    {
        // Line 123: Collection Initializers (C# 3.0 feature)
        return new List<InventoryItem>
        {
            new InventoryItem(101, "Laptop", "Electronics", 15, 999.99m, new DateTime(2025, 10, 20)),
            new InventoryItem(102, "Mouse", "Electronics", 150, 15.50m, new DateTime(2025, 11, 01)),
            new InventoryItem(103, "Keyboard", "Electronics", 80, 45.00m, new DateTime(2025, 09, 15)),
            new InventoryItem(201, "T-Shirt", "Apparel", 250, 19.99m, new DateTime(2025, 08, 10)),
            new InventoryItem(202, "Jeans", "Apparel", 45, 49.99m, new DateTime(2024, 12, 05)),
            new InventoryItem(203, "Jacket", "Apparel", 10, 89.99m, new DateTime(2025, 07, 25)),
            new InventoryItem(301, "Notebook", "Office", 300, 3.50m, new DateTime(2025, 05, 01)),
            new InventoryItem(302, "Pen Set", "Office", 30, 12.99m, new DateTime(2025, 10, 15)),
            new InventoryItem(303, "Monitor", "Electronics", 5, 299.99m, new DateTime(2025, 11, 05), true), // Discontinued
            new InventoryItem(401, "Coffee Maker", "Appliances", 60, 75.00m, new DateTime(2024, 11, 28)),
            new InventoryItem(402, "Toaster", "Appliances", 15, 30.00m, new DateTime(2025, 10, 30)),
            new InventoryItem(403, "Blender", "Appliances", 110, 45.99m, new DateTime(2025, 02, 14)),
            new InventoryItem(501, "Socks", "Apparel", 50, 5.99m, new DateTime(2025, 03, 10))
        };
    }

    // Line 146: Helper method for printing
    private static void PrintItems(IEnumerable<InventoryItem> items)
    {
        if (!items.Any()) // Using a LINQ method for checking emptiness
        {
            Console.WriteLine("[No items found]");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}

// Line 160: Static Class for Extension Methods (C# 3.0 feature)
public static class InventoryExtensions
{
    // Line 164: Extension Method for IEnumerable<InventoryItem>
    // This allows calling it directly on the collection: inventory.RestockedAfter(...)
    public static IEnumerable<InventoryItem> RestockedAfter(this IEnumerable<InventoryItem> source, DateTime date)
    {
        // Line 168: Use a lambda expression within the Where extension method
        return source.Where(item => item.LastRestockDate >= date && item.IsDiscontinued == false);
    }

    // Line 173: Another useful extension method
    public static IEnumerable<InventoryItem> InCategory(this IEnumerable<InventoryItem> source, string categoryName)
    {
        // Line 177: Standard query operator
        return source.Where(item => item.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
    }
}

// Line 182: Console Output Simulation (Approximate output length: 20 lines)
/*
--- Full Inventory Listing ---
...
--- 1. Low Stock Items (< 50) using Query Syntax ---
...
--- 2. High Value Products (> $25) using Method Syntax (Lambda) ---
...
--- 3. Grouped by Category and Total Value (Mixed Syntax) ---
...
--- 4. Items Restocked This Year (Using an Extension Method) ---
...
--- 5. First 3 Items in 'Electronics' Category ---
...
*/
// Line 200: Code ends here.
