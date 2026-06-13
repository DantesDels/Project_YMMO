using AutoMapper;
using YMMO.Backend.Application.DTOs.Authentification;
using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class AuthClientMappingProfile : Profile
{
    public AuthClientMappingProfile()
    {
        CreateMap<Client, ClientProfileDto>()
            .ForMember(dest => dest.ActiveOffersCount, opt => opt.Ignore());

        CreateMap<AuthentificationDto.RegisterRequest, Client>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Criteria, opt => opt.Ignore())
            .ForMember(dest => dest.AgentId, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedAgent, opt => opt.Ignore())
            .ForMember(dest => dest.OwnedProperties, opt => opt.Ignore())
            .ForMember(dest => dest.BoughtProperties, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForMember(dest => dest.WishlistItems, opt => opt.Ignore())
            .ForMember(dest => dest.ContactId, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.ContactRole, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}