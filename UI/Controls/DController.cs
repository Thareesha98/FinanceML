using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// --- MODEL DEFINITIONS (Simplified DTOs for API) ---

// DTO for receiving data from the client
public class DataSubmissionDto
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string DataPayload { get; set; }
}

// DTO for returning a processed result to the client
public class ResultResponseDto
{
    public int UserId { get; set; }
    public decimal FinalScore { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}

// --- API CONTROLLER IMPLEMENTATION ---

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataProcessorService _processorService;
    private readonly ILogger<DataController> _logger;

    public DataController(IDataProcessorService processorService, ILogger<DataController> logger)
    {
        _processorService = processorService ?? throw new ArgumentNullException(nameof(processorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("DataController initialized and ready to handle requests.");
    }

    /// <summary>
    /// POST endpoint to process a single piece of raw data.
    /// Route: POST api/data/process
    /// </summary>
    /// <param name="data">The raw data input from the client.</param>
    /// <returns>A 200 OK with the processed result or a 400/500 status code.</returns>
    [HttpPost("process")]
    [ProducesResponseType(typeof(ResultResponseDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> ProcessSingleData([FromBody] DataSubmissionDto data)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state received for single data processing.");
            return BadRequest(ModelState);
        }

        // Convert DTO to internal model (RawDataInput)
        var rawInput = new RawDataInput
        {
            UserId = data.UserId,
            Amount = data.Amount,
            DataPayload = data.DataPayload,
            Timestamp = DateTime.UtcNow // Set server-side timestamp
        };

        try
        {
            _logger.LogDebug("Received data for processing for User ID: {UserId}", data.UserId);

            // Delegate core logic to the business service layer
            var processedResult = await _processorService.ProcessDataAsync(rawInput);

            // Convert internal result model to response DTO
            var response = new ResultResponseDto
            {
                UserId = processedResult.UserId,
                FinalScore = processedResult.FinalScore,
                Status = processedResult.Status,
                Message = processedResult.Status == "LowRisk" ? "Data processed successfully." : "Review required."
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Client input failed validation for user {UserId}.", data.UserId);
            // Return 400 Bad Request for client-side errors
            return BadRequest(ex.Message);
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, "Business logic failure during processing for user {UserId}.", data.UserId);
            // Return 500 Internal Server Error for unhandled business exceptions
            return StatusCode(500, "A problem occurred during processing. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected critical error in DataController for user {UserId}.", data.UserId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// POST endpoint to process a batch of data entries.
    /// Route: POST api/data/batch
    /// </summary>
    /// <param name="dataBatch">A list of raw data inputs.</param>
    /// <returns>A 200 OK with the results of the batch processing.</returns>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(IEnumerable<ResultResponseDto>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> ProcessDataBatch([FromBody] List<DataSubmissionDto> dataBatch)
    {
        if (dataBatch == null || dataBatch.Count == 0)
        {
            return BadRequest("The batch request body cannot be empty.");
        }

        _logger.LogInformation("Received batch of {Count} items for processing.", dataBatch.Count);

        // Map DTOs to internal models
        var rawInputs = dataBatch.Select(d => new RawDataInput
        {
            UserId = d.UserId,
            Amount = d.Amount,
            DataPayload = d.DataPayload,
            Timestamp = DateTime.UtcNow
        }).ToList();

        try
        {
            var processedResults = await _processorService.ProcessBatchAsync(rawInputs);

            // Map internal results to response DTOs
            var responseBatch = processedResults.Select(r => new ResultResponseDto
            {
                UserId = r.UserId,
                FinalScore = r.FinalScore,
                Status = r.Status,
                Message = r.Status == "BatchFailure" ? "Item failed in batch processing." : "Item processed."
            });

            // Return 200 OK even if some items failed, as the batch itself was handled
            return Ok(responseBatch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error while processing batch of {Count} items.", dataBatch.Count);
            return StatusCode(500, "A critical server error halted the batch processing.");
        }
    }
}
