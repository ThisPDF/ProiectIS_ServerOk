using OKServer.Models;

namespace OKServer.Repositories;

public interface IHeartbeatRepository
{
    Task<IEnumerable<Heartbeat>> GetByDeviceIdAsync(Guid deviceId);
    Task<Heartbeat?> GetByIdAsync(Guid id);
    Task<Heartbeat> CreateAsync(Heartbeat heartbeat);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

