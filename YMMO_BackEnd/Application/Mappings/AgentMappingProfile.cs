using AutoMapper;
using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class AgentMappingProfile : Profile
{
    public AgentMappingProfile()
    {
        CreateMap<CreateAgentDto, Agent>()
            .ForMember(dest => dest.Agency, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedClients, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedOffers, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore())
            .ForMember(dest => dest.ContactId, opt => opt.Ignore())
            .ForMember(dest => dest.ContactRole, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<UpdateAgentDto, Agent>()
            .ForMember(dest => dest.Agency, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedClients, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedOffers, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore())
            .ForMember(dest => dest.ContactId, opt => opt.Ignore())
            .ForMember(dest => dest.ContactRole, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<Agent, AgentContactDto>();
    }
}