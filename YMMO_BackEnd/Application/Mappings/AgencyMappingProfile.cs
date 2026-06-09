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
        CreateMap<LocationDto, Location>();
        
        CreateMap<UpdateAgencyDto, Agency>()
            .ForMember(dest => dest.Location, opt 
                => opt.Ignore());
        
    }
}