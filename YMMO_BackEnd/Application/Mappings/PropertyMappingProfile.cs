using AutoMapper;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<Property, PropertySummaryDto>();
        CreateMap<Property, PropertyDetailDto>();
        CreateMap<CreatePropertyDto, Property>();
        CreateMap<UpdatePropertyDto, Property>();
    }
}