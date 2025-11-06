using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncProcessor
{
    /// <summary>
    /// A simple thread-safe logger to handle console output from multiple tasks.
    /// </summary>
    public static class ThreadSafeLogger
    {
        /// <summary>
        /// A private object used for locking to ensure only one thread
        /// writes to the console at a time.
        /// </summary>
        private static readonly object _consoleLock = new object();

        /// <summary>
        /// Logs a message to the console in a thread-safe manner.
        /// Prepends the current managed thread ID to the message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            // Lock the console to prevent garbled output
            // from multiple threads writing at the same time.
            lock (_consoleLock)
            {
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId:D2}] {message}");
            }
        }

        /// <summary>
        /// Logs a message with a specific color.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="color">The console color to use.</param>
        public static void Log(string message, ConsoleColor color)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId:D2}] {message}");
                Console.ResetColor();
            }
        }
    }

    // ######################################################################

    /// <summary>
    /// Represents the result of a data processing operation.
    /// </summary>
    public class ProcessingResult
    {
        /// <summary>
        /// The original data source (e.g., URL or ID).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The processed data payload.
        /// </summary>
        public string ProcessedData { get; set; }

        /// <summary>
        /// The time taken to process this item, in milliseconds.
        /// </summary>
        public long TimeTakenMs { get; set; }

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Error message, if any.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProcessingResult()
        {
            Source = string.Empty;
            ProcessedData = string.Empty;
            ErrorMessage = string.Empty;
        }
    }

    // ######################################################################

    /// <summary>
    /// Simulates fetching and processing data from various sources.
    /// </summary>
    public class DataProcessor
    {
        /// <summary>
        /// A shared Random instance for simulating variable network/processing time.
        /// </summary>
        private static readonly Random _random = new Random();

        /// <summary>
        /// Simulates an asynchronous data-fetching operation (e.g., an HTTP GET request).
        /// </summary>
        /// <param name="dataSource">A string representing the data source (e.g., a URL).</param>
        /// <param name="token">A cancellation token to observe.</param>
        /// <returns>A string representing the "raw data" fetched.</returns>
        private async Task<string> FetchDataAsync(string dataSource, CancellationToken token)
        {
            ThreadSafeLogger.Log($"FETCHING from {dataSource}...");

            // Simulate network latency (1 to 3 seconds)
            int delayMs = _random.Next(1000, 3001);
            await Task.Delay(delayMs, token);

            // Check if cancellation was requested *during* the delay
            token.ThrowIfCancellationRequested();

            ThreadSafeLogger.Log($"FETCHED from {dataSource}. (Took {delayMs}ms)", ConsoleColor.Green);

            // Return some dummy raw data
            return $"\"Raw data payload for {dataSource}\"";
        }

        /// <summary>
        /// Simulates an asynchronous data-processing operation (e.g., parsing JSON, calculations).
        /// </summary>
        /// <param name="rawData">The raw data to process.</param>
        /// <param name="token">A cancellation token to observe.</param>
        /// <returns>A string representing the "processed data".</returns>
        private async Task<string> ProcessDataAsync(string rawData, CancellationToken token)
        {
            ThreadSafeLogger.Log($"PROCESSING data: {rawData.Substring(0, 20)}...");

            // Simulate CPU-bound work (0.5 to 1.5 seconds)
            int delayMs = _random.Next(500, 1501);
            await Task.Delay(delayMs, token);

            // Check if cancellation was requested
            token.ThrowIfCancellationRequested();

            ThreadSafeLogger.Log($"PROCESSED data: {rawData.Substring(0, 20)}... (Took {delayMs}ms)", ConsoleColor.Cyan);

            // Return some dummy processed data (e.g., "unwrapping" the quotes)
            return rawData.Trim('"').ToUpper();
        }

        /// <summary>
        /// The main pipeline method that orchestrates fetching and processing for a single item.
        /// </summary>
        /// <param name="dataSource">The data source to process.</param>
        /// <param name="token">A cancellation token to observe.</param>
        /// <returns>A ProcessingResult object.</returns>
        public async Task<ProcessingResult> FetchAndProcessDataAsync(string dataSource, CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new ProcessingResult { Source = dataSource };

            try
            {
                // Step 1: Fetch
                string rawData = await FetchDataAsync(dataSource, token);

                // Ensure we haven't been cancelled between steps
                token.ThrowIfCancellationRequested();

                // Step 2: Process
                string processedData = await ProcessDataAsync(rawData, token);

                // Step 3: Populate successful result
                result.ProcessedData = processedData;
                result.WasSuccessful = true;
                ThreadSafeLogger.Log($"COMPLETED: {dataSource}", ConsoleColor.Yellow);
            }
            catch (OperationCanceledException)
            {
                // This is expected if the token is cancelled
                result.WasSuccessful = false;
                result.ErrorMessage = "Operation was cancelled.";
                ThreadSafeLogger.Log($"CANCELLED: {dataSource}", ConsoleColor.DarkRed);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                result.WasSuccessful = false;
                result.ErrorMessage = ex.Message;
                ThreadSafeLogger.Log($"FAILED: {dataSource} - {ex.Message}", ConsoleColor.Red);
            }
            finally
            {
                stopwatch.Stop();
                result.TimeTakenMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }
    }

    // ######################################################################

    /// <summary>
    /// Main program class containing the application entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for the async simulator.
        /// </summary>
        static async Task Main(string[] args)
        {
            Console.Title = "Async Data Processor";
            ThreadSafeLogger.Log("======= Starting Async Data Processing Job =======", ConsoleColor.White);

            var processor = new DataProcessor();

            // Create a CancellationTokenSource to allow for cancellation
            // e.g., if the job takes too long.
            var cts = new CancellationTokenSource();

            // --- Set up a timeout to cancel the operation ---
            // We can cancel via a timeout or by pressing a key
            Console.WriteLine("\nPress 'C' to cancel the operation or wait 10 seconds for timeout...\n");

            // Start a task to listen for key press
            var keyPressCancelTask = Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    ThreadSafeLogger.Log("!!! 'C' key pressed. Requesting cancellation... !!!", ConsoleColor.Red);
                    cts.Cancel();
                }
            });

            // Set a 10-second timeout
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            // ------------------------------------------------

            // A list of dummy data sources to process
            var dataSources = new List<string>
            {
                "api.example.com/data/1",
                "api.example.com/data/2",
                "storage.blob.core.windows.net/files/report.csv",
                "api.example.com/data/3",
                "ftp.example.com/data.zip",
                "api.example.com/data/4",
                "api.example.com/data/5",
                "api.example.com/data/6"
            };

            // Create a list to hold all the running tasks
            var processingTasks = new List<Task<ProcessingResult>>();

            var mainStopwatch = Stopwatch.StartNew();

            // --- Start all tasks concurrently ---
            ThreadSafeLogger.Log($"Starting {dataSources.Count} processing tasks in parallel...", ConsoleColor.White);
            foreach (var source in dataSources)
            {
                // Create and start the task, adding it to the list
                processingTasks.Add(
                    processor.FetchAndProcessDataAsync(source, cts.Token)
                );
            }

            // --- Wait for all tasks to complete ---
            // Task.WhenAll aggregates all tasks into a single task
            // that completes when all of its constituent tasks have completed.
            ProcessingResult[] results = await Task.WhenAll(processingTasks);
            
            mainStopwatch.Stop();
            ThreadSafeLogger.Log($"\n======= Job Finished in {mainStopwatch.Elapsed.TotalSeconds:F2} seconds =======", ConsoleColor.White);

            // --- Display Summary Report ---
            Console.WriteLine("\n--- Processing Summary ---");

            int successCount = 0;
            int failedCount = 0;
            int cancelledCount = 0;

            foreach (var res in results.OrderBy(r => r.Source))
            {
                if (res.WasSuccessful)
                {
                    successCount++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[SUCCESS] {res.Source} -> {res.ProcessedData} ({res.TimeTakenMs}ms)");
                }
                else if (res.ErrorMessage.Contains("cancelled"))
                {
                    cancelledCount++;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[CANCELLED] {res.Source} ({res.TimeTakenMs}ms)");
                }
                else
                {
                    failedCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAILED] {res.Source} -> {res.ErrorMessage} ({res.TimeTakenMs}ms)");
                }
            }
            Console.ResetColor();

            // Final summary line
            Console.WriteLine("\n--------------------------");
            Console.WriteLine($"Total: {results.Length} | Success: {successCount} | Failed: {failedCount} | Cancelled: {cancelledCount}");
            Console.WriteLine("--------------------------");
            
            // Wait for the key press task to finish (if it's still running)
            // and then for a final key press to exit.
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }
}
