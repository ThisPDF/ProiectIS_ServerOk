using AutoMapper;
using OKServer.DTOs;
using OKServer.Models;
using OKServer.Repositories;

namespace OKServer.Services;

public class HeartbeatService : IHeartbeatService
{
    private readonly IHeartbeatRepository _heartbeatRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IMapper _mapper;

    public HeartbeatService(
        IHeartbeatRepository heartbeatRepository,
        IDeviceRepository deviceRepository,
        IMapper mapper)
    {
        _heartbeatRepository = heartbeatRepository;
        _deviceRepository = deviceRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HeartbeatDto>> GetByDeviceIdAsync(Guid deviceId)
    {
        var heartbeats = await _heartbeatRepository.GetByDeviceIdAsync(deviceId);
        return _mapper.Map<IEnumerable<HeartbeatDto>>(heartbeats);
    }

    public async Task<HeartbeatDto?> GetByIdAsync(Guid id)
    {
        var heartbeat = await _heartbeatRepository.GetByIdAsync(id);
        return heartbeat == null ? null : _mapper.Map<HeartbeatDto>(heartbeat);
    }

    public async Task<HeartbeatDto> CreateAsync(Guid deviceId, CreateHeartbeatDto createDto)
    {
        // Verify device exists
        if (!await _deviceRepository.ExistsAsync(deviceId))
        {
            throw new ArgumentException($"Device with id {deviceId} does not exist", nameof(deviceId));
        }

        var heartbeat = _mapper.Map<Heartbeat>(createDto);
        heartbeat.Id = Guid.NewGuid();
        heartbeat.DeviceId = deviceId;
        heartbeat.Timestamp = DateTime.UtcNow;
        
        var createdHeartbeat = await _heartbeatRepository.CreateAsync(heartbeat);
        return _mapper.Map<HeartbeatDto>(createdHeartbeat);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _heartbeatRepository.DeleteAsync(id);
    }
}

