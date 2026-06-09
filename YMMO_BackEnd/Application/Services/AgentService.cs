using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.BackEnd.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class AgentService : IAgentService
{
    private readonly IYmmoDbContext _context;
    private readonly IAgentRepository _agentRepository;
    private readonly ILogger<AgentService> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public AgentService(IYmmoDbContext context, IAgentRepository agentRepository, ILogger<AgentService> logger,  IPasswordHasher passwordHasher)
    {
        _context = context;
        _agentRepository = agentRepository;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<AgentContactDto> CreateAgentAsync(CreateAgentDto dto)
    {
        var newAgent = new Agent
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            AgencyID = dto.AgencyID ?? throw new ArgumentNullException(nameof(dto.AgencyID)),
            PasswordHash =  _passwordHasher.Hash(dto.Password)
        };

        await _agentRepository.AddAsync(newAgent);
    
        return new AgentContactDto {
            FirstName = newAgent.FirstName,
            LastName = newAgent.LastName,
            Email = newAgent.Email,
            PhoneNumber = newAgent.PhoneNumber
        };
    }
    
    public async Task UpdateAgentProfileAsync(Guid agentId, UpdateAgentDto dto)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        
        if (agent == null)
        {
            _logger.LogWarning("Attempted to update non-existent agent with ID {AgentId}.", agentId);
            throw new KeyNotFoundException("Agent introuvable.");
        }

        // Update fields
        agent.FirstName = dto.FirstName;
        agent.LastName = dto.LastName;
        agent.Email = dto.Email;
        agent.PhoneNumber = dto.PhoneNumber;

        // Persist changes
        await _agentRepository.UpdateAsync(agent);
        _logger.LogInformation("Agent {AgentId} mis à jour avec succès.", agentId);
    }

    public async Task<AgentContactDto> GetAgentDetailsAsync(Guid agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        
        if (agent == null)
        {
            _logger.LogWarning("Agent with ID {AgentId} not found.", agentId);
            throw new KeyNotFoundException("Agent introuvable.");
        }

        return new AgentContactDto
        {
            FirstName = agent.FirstName,
            LastName = agent.LastName,
            Email = agent.Email,
            PhoneNumber = agent.PhoneNumber
        };
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetAgentPropertiesAsync(Guid agentId)
    {
        _logger.LogInformation("Fetching properties for agent {AgentId}", agentId);

        return await _context.Properties
            .Where(p => p.AgentID == agentId)
            .Select(p => new PropertySummaryDto 
            { 
                PropertyID = p.PropertyID,
                City = p.Location.City ?? "Inconnu",
                CurrentPrice = p.CurrentPrice // Mapping from your entity's CurrentPrice
            })
            .ToListAsync();
    }
}