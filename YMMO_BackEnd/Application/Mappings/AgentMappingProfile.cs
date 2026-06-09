using AutoMapper;
using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Mappings;

public class AgentMappingProfile : Profile
{
    public AgentMappingProfile()
    {
        CreateMap<CreateAgentDto, Agent>();
        CreateMap<UpdateAgentDto, Agent>();
        CreateMap<Agent, AgentContactDto>();
    }
}