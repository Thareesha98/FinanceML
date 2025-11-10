// Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TaskScheduler
{
    // 1. Custom Class Definition (Meaningful Data Structure)
    public class UserTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        public override string ToString()
        {
            var status = IsCompleted ? "[DONE]" : "[PENDING]";
            return $"[{DueDate:yyyy-MM-dd}] {status} - {Title}";
        }
    }

    public class TaskManager
    {
        private List<UserTask> Tasks { get; set; } = new List<UserTask>();
        private const string DataFilePath = "tasks.json";

        public TaskManager()
        {
            LoadTasks(); // Load data when manager is instantiated
        }

        // 2. File I/O for Persistence (JSON Serialization)
        private void LoadTasks()
        {
            if (File.Exists(DataFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(DataFilePath);
                    Tasks = JsonSerializer.Deserialize<List<UserTask>>(jsonString) ?? new List<UserTask>();
                    Console.WriteLine($"\nLoaded {Tasks.Count} tasks from file.");
                }
                catch (Exception ex)
                {
                    // Basic error handling for file operations
                    Console.WriteLine($"Error loading tasks: {ex.Message}");
                    Tasks = new List<UserTask>();
                }
            }
        }

        public void SaveTasks()
        {
            var jsonString = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFilePath, jsonString);
            Console.WriteLine("\nTasks saved successfully.");
        }

        // 3. Core Business Logic - Adding a Task
        public void AddTask(string title, DateTime dueDate)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Task title cannot be empty.");
            }

            Tasks.Add(new UserTask { Title = title, DueDate = dueDate });
            Console.WriteLine("Task added.");
        }

        // 4. Data Retrieval with LINQ
        public void DisplayPendingTasks()
        {
            Console.WriteLine("\n--- PENDING TASKS ---");
            // LINQ Query: Filter where IsCompleted is false and order by DueDate
            var pending = Tasks
                .Where(t => !t.IsCompleted)
                .OrderBy(t => t.DueDate);

            if (!pending.Any())
            {
                Console.WriteLine("No pending tasks! 🎉");
                return;
            }

            int index = 1;
            foreach (var task in pending)
            {
                Console.WriteLine($"{index++}. {task}");
            }
        }

        public void MarkTaskComplete(int listIndex)
        {
            // Get the list of pending tasks in the same order as DisplayPendingTasks
            var pending = Tasks
                .Where(t => !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToList();

            if (listIndex > 0 && listIndex <= pending.Count)
            {
                pending[listIndex - 1].IsCompleted = true;
                Console.WriteLine($"\nTask '{pending[listIndex - 1].Title}' marked as complete.");
            }
            else
            {
                Console.WriteLine("\nInvalid task number.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Simple C# Task Scheduler ---");
            var manager = new TaskManager();
            string command;

            do
            {
                Console.WriteLine("\nAvailable Commands: ADD, VIEW, COMPLETE, EXIT");
                Console.Write("> ");
                command = Console.ReadLine()?.Trim().ToUpper();

                try
                {
                    switch (command)
                    {
                        case "ADD":
                            Console.Write("Enter task title: ");
                            var title = Console.ReadLine();
                            Console.Write("Enter due date (YYYY-MM-DD, e.g., 2026-01-15): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                            {
                                manager.AddTask(title, date);
                                manager.SaveTasks();
                            }
                            else
                            {
                                Console.WriteLine("Invalid date format. Task not added.");
                            }
                            break;

                        case "VIEW":
                            manager.DisplayPendingTasks();
                            break;

                        case "COMPLETE":
                            manager.DisplayPendingTasks();
                            Console.Write("Enter the number of the task to complete: ");
                            if (int.TryParse(Console.ReadLine(), out int taskNum))
                            {
                                manager.MarkTaskComplete(taskNum);
                                manager.SaveTasks();
                            }
                            break;

                        case "EXIT":
                            Console.WriteLine("Exiting application.");
                            break;

                        default:
                            Console.WriteLine("Unknown command.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // General catch for unexpected errors
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                }

            } while (command != "EXIT");
        }
    }
}
