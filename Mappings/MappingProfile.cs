using AutoMapper;
using OKServer.DTOs;
using OKServer.Models;

namespace OKServer.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Device mappings
        CreateMap<Device, DeviceDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));
        CreateMap<CreateDeviceDto, Device>();
        CreateMap<UpdateDeviceDto, Device>();

        // Heartbeat mappings
        CreateMap<Heartbeat, HeartbeatDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.DeviceId, opt => opt.MapFrom(s => s.DeviceId.ToString()));
        CreateMap<CreateHeartbeatDto, Heartbeat>();
    }
}

