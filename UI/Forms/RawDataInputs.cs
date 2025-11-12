using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// --- DATA TRANSFER OBJECTS (DTOs) & MODELS ---

// Placeholder DTO for input data
public class RawDataInput
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string DataPayload { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Placeholder for the processed result
public class ProcessedResult
{
    public int UserId { get; set; }
    public decimal FinalScore { get; set; }
    public string Status { get; set; }
    public DateTime ProcessedTime { get; set; } = DateTime.UtcNow;
}

// Placeholder for the dependency (assuming IUserRepository from the previous answer)
public interface IUserRepository 
{
    Task<User> GetByIdAsync(int userId);
    // ... other methods ...
}

// --- DATA PROCESSOR SERVICE ---

public interface IDataProcessorService
{
    Task<ProcessedResult> ProcessDataAsync(RawDataInput input);
    Task<IEnumerable<ProcessedResult>> ProcessBatchAsync(IEnumerable<RawDataInput> inputs);
}

/// <summary>
/// Handles complex business logic involving data transformation and calculation.
/// </summary>
public class DataProcessorService : IDataProcessorService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DataProcessorService> _logger;
    private readonly AppConfig _config; // Injecting configuration

    public DataProcessorService(
        IUserRepository userRepository, 
        IConfigurationManager configManager,
        ILogger<DataProcessorService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = configManager.GetSettings();

        _logger.LogInformation("DataProcessorService initialized. API Key is: {Key}", _config.ExternalApi.ApiKey.Substring(0, 4) + "...");
    }

    /// <summary>
    /// Performs a multi-step process on a single piece of raw data.
    /// </summary>
    public async Task<ProcessedResult> ProcessDataAsync(RawDataInput input)
    {
        _logger.LogDebug("Starting processing for User ID: {UserId}", input.UserId);
        
        if (input == null || input.UserId <= 0)
        {
            _logger.LogError("Input data is invalid or missing User ID.");
            throw new ArgumentException("Invalid input data.");
        }

        try
        {
            // 1. Validation and Dependency Lookup
            var user = await _userRepository.GetByIdAsync(input.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User {UserId} not found or inactive. Cannot proceed.", input.UserId);
                return new ProcessedResult { UserId = input.UserId, FinalScore = 0, Status = "UserNotFound" };
            }
            
            // 2. Data Transformation/Cleaning
            decimal cleanedAmount = CleanAndNormalizeAmount(input.Amount, input.DataPayload);

            // 3. Core Calculation Logic
            decimal calculatedScore = CalculateScore(cleanedAmount, user.DateCreated);

            // 4. External Service Interaction (Simulated)
            string status = await CheckExternalRisk(calculatedScore);

            // 5. Final Result Mapping
            var result = new ProcessedResult
            {
                UserId = input.UserId,
                FinalScore = calculatedScore,
                Status = status
            };

            _logger.LogInformation("Successfully processed data for user {UserId}. Final Score: {Score}", input.UserId, calculatedScore);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during data processing for user {UserId}.", input.UserId);
            // Optionally, handle specific exceptions and re-throw wrapped exceptions
            throw new ApplicationException($"Failed to process data for user {input.UserId}.", ex);
        }
    }

    /// <summary>
    /// Processes a collection of data inputs concurrently.
    /// </summary>
    public async Task<IEnumerable<ProcessedResult>> ProcessBatchAsync(IEnumerable<RawDataInput> inputs)
    {
        if (inputs == null || !inputs.Any())
        {
            _logger.LogInformation("Received empty batch for processing.");
            return Enumerable.Empty<ProcessedResult>();
        }

        _logger.LogInformation("Processing batch of {Count} items.", inputs.Count());

        var processingTasks = inputs.Select(input => ProcessDataAsync(input)
            .ContinueWith(t => 
            {
                if (t.IsFaulted)
                {
                    // Log and return a default failure result for batch resilience
                    _logger.LogError(t.Exception, "A batch item failed processing: User ID {UserId}.", input.UserId);
                    return new ProcessedResult { UserId = input.UserId, Status = "BatchFailure" };
                }
                return t.Result;
            }, TaskScheduler.Default));

        var results = await Task.WhenAll(processingTasks);
        _logger.LogInformation("Batch processing complete. Successful items: {SuccessCount}", results.Count(r => r.Status != "BatchFailure"));
        return results;
    }

    // --- PRIVATE UTILITY METHODS (Where the line count can be expanded) ---

    private decimal CleanAndNormalizeAmount(decimal amount, string payload)
    {
        // Example logic: Ensure amount is positive and apply a factor based on payload content
        if (amount <= 0) amount = 0;
        
        // Complex payload parsing logic here
        decimal payloadFactor = payload.Length > 50 ? 1.0M : 0.9M;

        return amount * payloadFactor;
    }

    private decimal CalculateScore(decimal amount, DateTime userCreationDate)
    {
        // Example logic: Calculation based on input amount and user tenure
        int daysSinceCreation = (DateTime.UtcNow - userCreationDate).Days;
        decimal tenureFactor = Math.Min(1.0M, (decimal)daysSinceCreation / 365.0M); // Max factor of 1.0 after 1 year

        // Score formula: Amount adjusted by API setting and user tenure
        decimal score = amount * (1.0M / _config.ExternalApi.TimeoutMilliseconds) * tenureFactor * 1000;
        
        // Ensure result is non-negative and capped
        return Math.Max(0, Math.Round(score, 2));
    }

    private async Task<string> CheckExternalRisk(decimal score)
    {
        // Simulate an asynchronous call to an external API service
        await Task.Delay(50); 
        
        if (score > 1000)
        {
            return "HighRisk";
        }
        else if (score > 500)
        {
            return "MediumRisk";
        }
        return "LowRisk";
    }
}
