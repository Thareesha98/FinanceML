using System;

namespace MySimpleApp
{
    /// <summary>
    /// The main entry point for the console application.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Person Class Demo ---");

            // Create a new instance of the Person class
            Person person1 = new Person("Jane", "Doe");
            
            // Call a method on the person object
            Console.WriteLine($"Hello, {person1.GetFullName()}!");

            // Change the person's name using properties
            person1.FirstName = "John";
            Console.WriteLine($"Name changed to: {person1.GetFullName()}");
            
            Console.WriteLine(); // Add a blank line for spacing

            // --- Calculator Class Demo ---
            Console.WriteLine("--- Calculator Class Demo ---");

            double num1 = 10;
            double num2 = 5;

            // Call static methods on the Calculator class
            double sum = Calculator.Add(num1, num2);
            Console.WriteLine($"{num1} + {num2} = {sum}");

            double difference = Calculator.Subtract(num1, num2);
            Console.WriteLine($"{num1} - {num2} = {difference}");

            double product = Calculator.Multiply(num1, num2);
            Console.WriteLine($"{num1} * {num2} = {product}");

            // Handle division, which can cause errors
            try
            {
                double quotient = Calculator.Divide(num1, num2);
                Console.WriteLine($"{num1} / {num2} = {quotient}");

                // Try to divide by zero
                Console.WriteLine("Attempting to divide by zero...");
                Calculator.Divide(num1, 0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Wait for user input before closing
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }
}
