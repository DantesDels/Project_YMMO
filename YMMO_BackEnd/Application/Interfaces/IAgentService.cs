using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Application.DTOs.Property;

namespace YMMO.Backend.Application.Interfaces;

public interface IAgentService
{
    Task<AgentContactDto> GetAgentDetailsAsync(Guid agentId);
    Task<IEnumerable<PropertySummaryDto>> GetAgentPropertiesAsync(Guid agentId);
    Task UpdateAgentProfileAsync(Guid agentId, UpdateAgentDto dto);
}