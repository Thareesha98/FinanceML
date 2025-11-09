// XmlSerializer.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq; // X-Linq namespace

public class XmlInventorySerializer
{
    private const string FileName = "inventory_report.xml";
    
    public static void GenerateXmlReport(IEnumerable<InventoryItem> items)
    {
        Console.WriteLine("\n--- X-Linq XML Generation (C# 3.0 Object Initializers) ---");

        // Line 17: LINQ Query to select only the data needed for the report.
        var reportData = items
            .Where(item => item.IsDiscontinued == false)
            .OrderBy(item => item.Category)
            .Select(item => new // Anonymous Type for projection
            {
                item.ItemID,
                item.Name,
                item.Category,
                item.StockQuantity
            });

        // Line 30: Using X-Linq to construct the XML document.
        // This is a major C# 3.0 feature, making XML creation declarative.
        var inventoryXml = new XElement("InventoryReport",
            new XAttribute("GeneratedDate", DateTime.Now.ToShortDateString()),
            
            // Line 36: Iterating through the projected data and creating XElements.
            reportData.Select(i => new XElement("Item", 
                new XAttribute("ID", i.ItemID),
                // Line 40: Object Initializer syntax for XElement creation.
                new XElement("Name", i.Name),
                new XElement("Category", i.Category),
                new XElement("Quantity", i.StockQuantity)
            ))
        );

        // Line 47: Save the document (Simulated for this console example)
        // inventoryXml.Save(FileName);

        Console.WriteLine($"Successfully generated XML report (simulated save to {FileName}).");
        
        // Line 52: Display a snippet of the generated XML.
        Console.WriteLine("--- XML Snippet ---");
        Console.WriteLine(inventoryXml.ToString().Substring(0, 300) + "...");
    }
}
// Line 58
