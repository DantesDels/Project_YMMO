using AutoMapper;
using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class OfferMappingProfile : Profile
{
    public OfferMappingProfile()
    {
        CreateMap<CreateOfferDto, Offer>()
            .ForMember(dest => dest.OfferID, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
            .ForMember(dest => dest.DateModified, opt => opt.Ignore())
            .ForMember(dest => dest.DatePriceUpdated, opt => opt.Ignore())
            .ForMember(dest => dest.StatusOffer, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.AgentID, opt => opt.Ignore())
            .ForMember(dest => dest.Agent, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        CreateMap<Offer, OfferResponseDto>()
            .ForMember(dest => dest.ClientLastName, opt => opt.MapFrom(src => src.Client.LastName))
            .ForMember(dest => dest.ClientFirstName, opt => opt.MapFrom(src => src.Client.FirstName))
            .ForMember(dest => dest.ClientPhoneNumber, opt => opt.MapFrom(src => src.Client.PhoneNumber))
            .ForMember(dest => dest.PropertyCity, opt => opt.MapFrom(src => src.Property.Location.City))
            .ForMember(dest => dest.PropertyRegion, opt => opt.MapFrom(src => src.Property.Location.Region));
    }
}