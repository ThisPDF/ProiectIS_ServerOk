using OKServer.DTOs;

namespace OKServer.Services;

public interface IHeartbeatService
{
    Task<IEnumerable<HeartbeatDto>> GetByDeviceIdAsync(Guid deviceId);
    Task<HeartbeatDto?> GetByIdAsync(Guid id);
    Task<HeartbeatDto> CreateAsync(Guid deviceId, CreateHeartbeatDto createDto);
    Task<bool> DeleteAsync(Guid id);
}

