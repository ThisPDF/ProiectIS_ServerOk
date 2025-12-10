using OKServer.DTOs;

namespace OKServer.Services;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(Guid id);
    Task<DeviceDto> CreateAsync(CreateDeviceDto createDto);
    Task<DeviceDto?> UpdateAsync(Guid id, UpdateDeviceDto updateDto);
    Task<bool> DeleteAsync(Guid id);
}

