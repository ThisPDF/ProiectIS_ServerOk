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
    public async Task<ActionResult<IEnumerable<HeartbeatDto>>> GetByDeviceId(Guid deviceId)
    {
        var heartbeats = await _heartbeatService.GetByDeviceIdAsync(deviceId);
        return Ok(heartbeats);
    }

    [HttpPost("devices/{deviceId}/heartbeats")]
    public async Task<ActionResult<HeartbeatDto>> Create(Guid deviceId, [FromBody] CreateHeartbeatDto createDto)
    {
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
            var heartbeat = await _heartbeatService.CreateAsync(deviceId, createDto);
            return CreatedAtAction(nameof(GetById), new { id = heartbeat.Id }, heartbeat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("heartbeats/{id}")]
    public async Task<ActionResult<HeartbeatDto>> GetById(Guid id)
    {
        var heartbeat = await _heartbeatService.GetByIdAsync(id);
        if (heartbeat == null)
            return NotFound();

        return Ok(heartbeat);
    }

    [HttpDelete("heartbeats/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _heartbeatService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

