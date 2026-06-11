using AutoMapper;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.DTOs.PropertyPicture;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<Property, PropertySummaryDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Location.PostalCode))
            .ForMember(dest => dest.MainFeatures, opt => opt.Ignore());
        CreateMap<Property, PropertyDetailDto>();
        CreateMap<PropertyPicture, PropertyPictureDto>();
        
        CreateMap<CreatePropertyDto, Property>()
            // Ignore system-generated fields and complex relations during creation
            .ForMember(dest => dest.BuyerId, opt => opt.Ignore())
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.DateListed, opt => opt.Ignore())
            .ForMember(dest => dest.DateSold, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentPrice, opt => opt.Ignore())
            .ForMember(dest => dest.FinalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.Agency, opt => opt.Ignore())
            .ForMember(dest => dest.Agent, opt => opt.Ignore())
            .ForMember(dest => dest.LocationId, opt => opt.Ignore())
            .ForMember(dest => dest.Seller, opt => opt.Ignore())
            .ForMember(dest => dest.Buyer, opt => opt.Ignore())
            .ForMember(dest => dest.Pictures, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore());

        CreateMap<UpdatePropertyDto, Property>()
            // Ignore these fields so they are not overwritten during an update
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.Pictures, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
            .ForMember(dest => dest.DateListed, opt => opt.Ignore())
            .ForMember(dest => dest.DateSold, opt => opt.Ignore())
            .ForMember(dest => dest.InitialPrice, opt => opt.Ignore())
            .ForMember(dest => dest.FinalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
            .ForMember(dest => dest.YearBuilt, opt => opt.Ignore())
            .ForMember(dest => dest.Surface, opt => opt.Ignore())
            .ForMember(dest => dest.AgencyId, opt => opt.Ignore())
            .ForMember(dest => dest.Agency, opt => opt.Ignore())
            .ForMember(dest => dest.AgentId, opt => opt.Ignore())
            .ForMember(dest => dest.Agent, opt => opt.Ignore())
            .ForMember(dest => dest.LocationId, opt => opt.Ignore())
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.SellerId, opt => opt.Ignore())
            .ForMember(dest => dest.Seller, opt => opt.Ignore())
            .ForMember(dest => dest.BuyerId, opt => opt.Ignore())
            .ForMember(dest => dest.Buyer, opt => opt.Ignore())
            // Ensure we only map properties that are provided (not null) in the DTO
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}