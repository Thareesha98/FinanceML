using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManager
{
    /// <summary>
    /// Represents a single item in the inventory.
    /// This class is a simple data container (POCO).
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item in stock.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price of a single unit of the item.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InventoryItem()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Parameterized constructor for easy item creation.
        /// </summary>
        /// <param name="id">The item's ID.</param>
        /// <param name="name">The item's name.</param>
        /// <param name="quantity">The item's quantity.</param>
        /// <param name="price">The item's price.</param>
        public InventoryItem(int id, string name, int quantity, decimal price)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        /// <summary>
        /// Provides a string representation of the item.
        /// </summary>
        /// <returns>A formatted string with item details.</returns>
        public override string ToString()
        {
            return $"[ID: {Id}] {Name} - Qty: {Quantity}, Price: {Price:C}";
        }
    }

    // ######################################################################

    /// <summary>
    /// Manages the collection of inventory items.
    /// Handles adding, removing, updating, and displaying items.
    /// Also handles saving and loading the inventory to/from a file.
    /// </summary>
    public class Inventory
    {
        private List<InventoryItem> _items;
        private int _nextId;

        /// <summary>
        /// Initializes a new instance of the Inventory class.
        /// </summary>
        public Inventory()
        {
            _items = new List<InventoryItem>();
            _nextId = 1; // Start ID counter
        }

        /// <summary>
        /// Adds a new item to the inventory.
        /// </summary>
        /// <param name="name">Name of the new item.</param>
        /// <param name="quantity">Quantity of the new item.</param>
        /// <param name="price">Price of the new item.</param>
        public void AddItem(string name, int quantity, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Error: Item name cannot be empty.");
                return;
            }
            if (quantity < 0)
            {
                Console.WriteLine("Error: Quantity cannot be negative.");
                return;
            }
            if (price < 0)
            {
                Console.WriteLine("Error: Price cannot be negative.");
                return;
            }

            var newItem = new InventoryItem(_nextId, name, quantity, price);
            _items.Add(newItem);
            _nextId++; // Increment ID for the next item

            Console.WriteLine($"Successfully added: {newItem}");
        }

        /// <summary>
        /// Removes an item from the inventory based on its ID.
        /// </summary>
        /// <param name="id">The ID of the item to remove.</param>
        public void RemoveItem(int id)
        {
            var itemToRemove = FindItemById(id);
            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);
                Console.WriteLine($"Successfully removed: {itemToRemove.Name}");
            }
            else
            {
                Console.WriteLine($"Error: Item with ID {id} not found.");
            }
        }

        /// <summary>
        /// Updates the quantity of an existing item.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="newQuantity">The new quantity.</param>
        public void UpdateItemQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                Console.WriteLine("Error: Quantity cannot be negative.");
                return;
            }

            var itemToUpdate = FindItemById(id);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = newQuantity;
                Console.WriteLine($"Successfully updated quantity for: {itemToUpdate.Name}");
                Console.WriteLine($"New details: {itemToUpdate}");
            }
            else
            {
                Console.WriteLine($"Error: Item with ID {id} not found.");
            }
        }

        /// <summary>
        /// Finds a single item by its ID.
        /// </summary>
        /// <param name="id">The ID to search for.</param>
        /// <returns>The InventoryItem if found; otherwise, null.</returns>
        public InventoryItem FindItemById(int id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Displays all items currently in the inventory.
        /// </summary>
        public void DisplayAllItems()
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("\nThe inventory is currently empty.");
                return;
            }

            Console.WriteLine("\n--- Current Inventory ---");
            foreach (var item in _items)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("-------------------------");
        }

        /// <summary>
        /// Calculates and displays the total value of the inventory.
        /// </summary>
        public void DisplayTotalInventoryValue()
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("\nInventory is empty. Total value is $0.00.");
                return;
            }

            decimal totalValue = 0;
            foreach (var item in _items)
            {
                totalValue += item.Price * item.Quantity;
            }

            Console.WriteLine($"\nTotal inventory value: {totalValue:C}");
            Console.WriteLine($"Total items: {_items.Count}");
        }

        /// <summary>
        /// Saves the current inventory to a JSON file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to save to.</param>
        public async Task SaveToFileAsync(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Pretty-print the JSON
                };

                // Use 'await using' for proper async disposal of the stream
                await using (FileStream createStream = File.Create(filePath))
                {
                    await JsonSerializer.SerializeAsync(createStream, _items, options);
                }

                Console.WriteLine($"Inventory successfully saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the inventory from a JSON file asynchronously.
        /// This will overwrite the current in-memory inventory.
        /// </summary>
        /// <param name="filePath">The path of the file to load from.</param>
        public async Task LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}. Starting with an empty inventory.");
                _items = new List<InventoryItem>();
                _nextId = 1;
                return;
            }

            try
            {
                // Use 'await using' for proper async disposal of the stream
                await using (FileStream openStream = File.OpenRead(filePath))
                {
                    var loadedItems = await JsonSerializer.DeserializeAsync<List<InventoryItem>>(openStream);

                    if (loadedItems != null)
                    {
                        _items = loadedItems;
                        // Update the _nextId to be one higher than the max ID in the loaded list
                        _nextId = _items.Any() ? _items.Max(item => item.Id) + 1 : 1;
                        Console.WriteLine($"Inventory successfully loaded from {filePath}");
                    }
                    else
                    {
                        Console.WriteLine("File contained no data. Starting fresh.");
                        _items = new List<InventoryItem>();
                        _nextId = 1;
                    }
                }
            }
            catch (JsonException jEx)
            {
                Console.WriteLine($"Error deserializing file (file may be corrupt): {jEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // ######################################################################

    /// <summary>
    /// Main program class containing the application entry point.
    /// </summary>
    class Program
    {
        // Static inventory instance and file path
        private static readonly Inventory _inventory = new Inventory();
        private const string FilePath = "inventory.json";

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static async Task Main(string[] args)
        {
            Console.Title = "Inventory Management System";
            await _inventory.LoadFromFileAsync(FilePath); // Load data on startup

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        _inventory.DisplayAllItems();
                        break;
                    case "2":
                        HandleAddItem();
                        break;
                    case "3":
                        HandleRemoveItem();
                        break;
                    case "4":
                        HandleUpdateQuantity();
                        break;
                    case "5":
                        _inventory.DisplayTotalInventoryValue();
                        break;
                    case "6":
                        await _inventory.SaveToFileAsync(FilePath);
                        break;
                    case "7":
                        await _inventory.LoadFromFileAsync(FilePath);
                        break;
                    case "8":
                        exit = true;
                        Console.WriteLine("Exiting program. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select from 1-8.");
                        break;
                }
                PauseForUser();
            }
        }

        /// <summary>
        /// Displays the main menu options to the console.
        /// </summary>
        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("  Inventory Management System Menu  ");
            Console.WriteLine("====================================");
            Console.WriteLine("1. View All Items");
            Console.WriteLine("2. Add a New Item");
            Console.WriteLine("3. Remove an Item (by ID)");
            Console.WriteLine("4. Update Item Quantity (by ID)");
            Console.WriteLine("5. View Total Inventory Value");
            Console.WriteLine("6. Save Inventory to File");
            Console.WriteLine("7. Load Inventory from File");
            Console.WriteLine("8. Exit");
            Console.WriteLine("------------------------------------");
            Console.Write("Enter your choice (1-8): ");
        }

        /// <summary>
        // Helper method to pause and wait for user to press Enter.
        /// </summary>
        private static void PauseForUser()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Handles the UI logic for adding a new item.
        /// </summary>
        private static void HandleAddItem()
        {
            try
            {
                Console.WriteLine("\n--- Add New Item ---");
                Console.Write("Enter item name: ");
                string name = Console.ReadLine() ?? string.Empty;

                Console.Write("Enter item quantity: ");
                int quantity = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Enter item price: ");
                decimal price = decimal.Parse(Console.ReadLine() ?? "0.00");

                _inventory.AddItem(name, quantity, price);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Quantity must be a number and price must be a decimal.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the UI logic for removing an item.
        /// </summary>
        private static void HandleRemoveItem()
        {
            try
            {
                Console.WriteLine("\n--- Remove Item ---");
                _inventory.DisplayAllItems();
                Console.Write("Enter the ID of the item to remove: ");
                int id = int.Parse(Console.ReadLine() ?? "0");
                _inventory.RemoveItem(id);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. ID must be a number.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the UI logic for updating an item's quantity.
        /// </summary>
        private static void HandleUpdateQuantity()
        {
            try
            {
                Console.WriteLine("\n--- Update Item Quantity ---");
                _inventory.DisplayAllItems();
                Console.Write("Enter the ID of the item to update: ");
                int id = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Enter the new quantity: ");
                int quantity = int.Parse(Console.ReadLine() ?? "0");

                _inventory.UpdateItemQuantity(id, quantity);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. ID and quantity must be numbers.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
