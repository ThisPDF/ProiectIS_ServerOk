using Microsoft.AspNetCore.Mvc;
using OKServer.DTOs;
using OKServer.Services;

namespace OKServer.Controllers;

[ApiController]
[Route("api/devices")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetById(Guid id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null)
            return NotFound();

        return Ok(device);
    }

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Create([FromBody] CreateDeviceDto createDto)
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
            var device = await _deviceService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the device", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DeviceDto>> Update(Guid id, [FromBody] UpdateDeviceDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                .ToList();
            return BadRequest(new { message = "Validation failed", errors = errors });
        }

        var device = await _deviceService.UpdateAsync(id, updateDto);
        if (device == null)
            return NotFound();

        return Ok(device);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _deviceService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

