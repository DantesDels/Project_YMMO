using AutoMapper;
using YMMO.Backend.Application.DTOs.Agency;
using YMMO.Backend.Application.DTOs.Location;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class AgencyMappingProfile : Profile
{
    public AgencyMappingProfile()
    {
        CreateMap<Agency, AgencyDto>();
        CreateMap<Location, LocationDto>();
        CreateMap<LocationDto, Location>()
            .ForMember(dest => dest.Complement, opt => opt.Ignore())
            .ForMember(dest => dest.LocationId, opt => opt.Ignore())
            .ForMember(dest => dest.Agencies, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore());
        
        CreateMap<UpdateAgencyDto, Agency>()
            .ForMember(dest => dest.AgencyId, opt => opt.Ignore())
            .ForMember(dest => dest.LocationId, opt => opt.Ignore())
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.Agents, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore());
    }
}