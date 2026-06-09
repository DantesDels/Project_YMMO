using AutoMapper;
using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class OfferMappingProfile : Profile
{
    public OfferMappingProfile()
    {
        CreateMap<CreateOfferDto, Offer>();
        CreateMap<Offer, OfferResponseDto>()
            .ForMember(dest => dest.ClientLastName, opt
                => opt.MapFrom(src => src.Client.LastName))
            .ForMember(dest => dest.ClientFirstName, opt
                => opt.MapFrom(src => src.Client.FirstName))
            .ForMember(dest => dest.PropertyCity, opt
                => opt.MapFrom(src => src.Property.Location.City))
            .ForMember(dest => dest.PropertyRegion, opt
                => opt.MapFrom(src => src.Property.Location.Region));
    }
}