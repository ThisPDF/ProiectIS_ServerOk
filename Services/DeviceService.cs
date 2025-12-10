using AutoMapper;
using OKServer.DTOs;
using OKServer.Models;
using OKServer.Repositories;

namespace OKServer.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repository;
    private readonly IMapper _mapper;

    public DeviceService(IDeviceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        var devices = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }

    public async Task<DeviceDto?> GetByIdAsync(Guid id)
    {
        var device = await _repository.GetByIdAsync(id);
        return device == null ? null : _mapper.Map<DeviceDto>(device);
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto createDto)
    {
        var device = _mapper.Map<Device>(createDto);
        device.Id = Guid.NewGuid();
        device.CreatedAt = DateTime.UtcNow;
        
        var createdDevice = await _repository.CreateAsync(device);
        return _mapper.Map<DeviceDto>(createdDevice);
    }

    public async Task<DeviceDto?> UpdateAsync(Guid id, UpdateDeviceDto updateDto)
    {
        var device = await _repository.GetByIdAsync(id);
        if (device == null)
            return null;

        _mapper.Map(updateDto, device);
        var updatedDevice = await _repository.UpdateAsync(device);
        return _mapper.Map<DeviceDto>(updatedDevice);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}

