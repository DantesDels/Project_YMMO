using AutoMapper;
using YMMO.Backend.Application.DTOs.Authentification;
using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class AuthClientMappingProfile : Profile
{
    public AuthClientMappingProfile()
    {
        CreateMap<Client, ClientProfileDto>();
        CreateMap<AuthentificationDto.RegisterRequest, Client>()
            .ForMember(dest => dest.FirstName, opt 
                => opt.MapFrom(src => src.Username));
    }
}