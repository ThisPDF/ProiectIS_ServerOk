using Microsoft.AspNetCore.Mvc;
using OKServer.DTOs;
using OKServer.Services;

namespace OKServer.Controllers;

[ApiController]
[Route("api")]
public class HeartbeatController : ControllerBase
{
    private readonly IHeartbeatService _heartbeatService;

    public HeartbeatController(IHeartbeatService heartbeatService)
    {
        _heartbeatService = heartbeatService;
    }

    [HttpGet("devices/{deviceId}/heartbeats")]
    public async Task<ActionResult<IEnumerable<HeartbeatDto>>> GetByDeviceId(string deviceId)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            return BadRequest("Invalid device ID format");
        
        var heartbeats = await _heartbeatService.GetByDeviceIdAsync(guid);
        return Ok(heartbeats);
    }

    [HttpPost("devices/{deviceId}/heartbeats")]
    public async Task<ActionResult<HeartbeatDto>> Create(string deviceId, [FromBody] CreateHeartbeatDto createDto)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            return BadRequest("Invalid device ID format");
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                .ToList();
            return BadRequest(new { message = "Validation failed", errors = errors });
        }

        try
        {
            var heartbeat = await _heartbeatService.CreateAsync(guid, createDto);
            return CreatedAtAction(nameof(GetById), new { id = heartbeat.Id }, heartbeat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("heartbeats/{id}")]
    public async Task<ActionResult<HeartbeatDto>> GetById(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return BadRequest("Invalid ID format");
        
        var heartbeat = await _heartbeatService.GetByIdAsync(guid);
        if (heartbeat == null)
            return NotFound();

        return Ok(heartbeat);
    }

    [HttpDelete("heartbeats/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return BadRequest("Invalid ID format");
        
        var deleted = await _heartbeatService.DeleteAsync(guid);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

