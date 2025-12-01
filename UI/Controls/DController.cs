using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ----------------------------
// DTO Definitions
// ----------------------------

public record DataInputDto(
    int UserId,
    decimal Amount,
    string Payload
);

public record DataResultDto(
    int UserId,
    decimal Score,
    string Status,
    string Message
);

// ----------------------------
// CONTROLLER IMPLEMENTATION
// ----------------------------

[ApiController]
[Route("api/v1/data")]
public class DataController : ControllerBase
{
    private readonly IDataProcessorService _processor;
    private readonly ILogger<DataController> _logger;

    public DataController(IDataProcessorService processor, ILogger<DataController> logger)
    {
        _processor = processor;
        _logger = logger;

        _logger.LogInformation("✔ DataController initialized.");
    }

    // ============================================================
    // PROCESS SINGLE ITEM
    // ============================================================

    [HttpPost("process")]
    [ProducesResponseType(typeof(DataResultDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> ProcessAsync([FromBody] DataInputDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid request payload.");
        }

        var input = new RawDataInput
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            DataPayload = dto.Payload,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            _logger.LogDebug("Received processing request for User {UserId}", dto.UserId);

            var result = await _processor.ProcessDataAsync(input);

            var response = new DataResultDto(
                result.UserId,
                result.FinalScore,
                result.Status,
                result.Status == "LowRisk" ? "Processed successfully." : "Further review required."
            );

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation failed for User {UserId}", dto.UserId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected server error while processing data");
            return StatusCode(500, "An unexpected server error occurred.");
        }
    }

    // ============================================================
    // PROCESS BATCH
    // ============================================================

    [HttpPost("batch")]
    [ProducesResponseType(typeof(IEnumerable<DataResultDto>), 200)]
    public async Task<IActionResult> ProcessBatchAsync([FromBody] IEnumerable<DataInputDto> batch)
    {
        if (batch == null || !batch.Any())
            return BadRequest("Batch cannot be empty.");

        _logger.LogInformation("Batch received → {Count} items", batch.Count());

        var inputs = batch.Select(dto => new RawDataInput
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            DataPayload = dto.Payload,
            Timestamp = DateTime.UtcNow
        }).ToList();

        try
        {
            var results = await _processor.ProcessBatchAsync(inputs);

            var response = results.Select(r => new DataResultDto(
                r.UserId,
                r.FinalScore,
                r.Status,
                r.Status == "BatchFailure" ? "Failed during batch execution." : "Processed."
            ));

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical failure in batch processing.");
            return StatusCode(500, "A fatal batch processing error occurred.");
        }
    }
}

