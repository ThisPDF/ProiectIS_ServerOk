using AutoMapper;
using OKServer.DTOs;
using OKServer.Models;

namespace OKServer.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Device mappings
        CreateMap<Device, DeviceDto>();
        CreateMap<CreateDeviceDto, Device>();
        CreateMap<UpdateDeviceDto, Device>();

        // Heartbeat mappings
        CreateMap<Heartbeat, HeartbeatDto>();
        CreateMap<CreateHeartbeatDto, Heartbeat>();
    }
}

