using OKServer.Models;

namespace OKServer.Repositories;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(Guid id);
    Task<Device> CreateAsync(Device device);
    Task<Device> UpdateAsync(Device device);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

