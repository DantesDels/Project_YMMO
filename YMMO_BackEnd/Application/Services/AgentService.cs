using AutoMapper;
using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Enums;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class AgentService : IAgentService
{
    private readonly IYmmoDbContext _context;
    private readonly IAgentRepository _agentRepository;
    private readonly ILogger<AgentService> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;
    private readonly IPropertyRepository _propertyRepository;

    public AgentService(
        IYmmoDbContext context, 
        IAgentRepository agentRepository, 
        ILogger<AgentService> logger,  
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IUserAccessor userAccessor,
        IPropertyRepository  propertyRepository
        )
    {
        _context = context;
        _agentRepository = agentRepository;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _userAccessor = userAccessor;
        _propertyRepository = propertyRepository;
    }
    
    public async Task<AgentContactDto> CreateAgentAsync(CreateAgentDto dto)
    {
        if (_userAccessor.GetCurrentUserRole() != ContactRole.Admin)
        {
            throw new UnauthorizedAccessException("Accès réservé aux administrateurs.");
        }

        var adminId = _userAccessor.GetCurrentUserId();
        var admin = await _agentRepository.GetByIdAsync(adminId);
        if (admin == null) throw new KeyNotFoundException("Admin introuvable.");

        var newAgent = _mapper.Map<Agent>(dto);
        
        newAgent.AgencyID = admin.AgencyID; 
        newAgent.SetRole(ContactRole.Agent); 
        newAgent.PasswordHash = _passwordHasher.Hash(dto.Password);

        await _agentRepository.AddAsync(newAgent);
        return _mapper.Map<AgentContactDto>(newAgent);
    }
    
    public async Task UpdateAgentProfileAsync(Guid agentId, UpdateAgentDto dto)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        
        if (agent == null)
        {
            _logger.LogWarning("Tentative de mise à jour agent inexistant {AgentId}.", agentId);
            throw new KeyNotFoundException("Agent introuvable.");
        }

        _mapper.Map(dto, agent);

        await _agentRepository.UpdateAsync(agent);
        _logger.LogInformation("Agent {AgentId} mis à jour avec succès.", agentId);
    }

    public async Task<AgentContactDto> GetAgentDetailsAsync(Guid agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        
        if (agent == null)
        {
            _logger.LogWarning("Agent {AgentId} non trouvé.", agentId);
            throw new KeyNotFoundException("Agent introuvable.");
        }

        return _mapper.Map<AgentContactDto>(agent);
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetAgentPropertiesAsync(Guid agentId)
    {
        var properties = await _propertyRepository.GetByIdAsync(agentId);
        return _mapper.Map<IEnumerable<PropertySummaryDto>>(properties);
    }
    
    public async Task DeleteAgentAsync(Guid agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null) throw new KeyNotFoundException("Agent introuvable.");

        await _agentRepository.DeleteAsync(agent);
        _logger.LogInformation("Agent {AgentId} supprimé.", agentId);
    }

    public async Task UpdatePasswordAsync(Guid agentId, string newPassword)
    {
        // Security : only admin or agent itself can change the password
        var currentUserId = _userAccessor.GetCurrentUserId();
        if (currentUserId != agentId && _userAccessor.GetCurrentUserRole() != ContactRole.Admin)
            throw new UnauthorizedAccessException("Non autorisé.");

        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null) throw new KeyNotFoundException("Agent introuvable.");

        agent.UpdatePassword(_passwordHasher.Hash(newPassword));
        await _agentRepository.UpdateAsync(agent);
        
        _logger.LogInformation("Mot de passe mis à jour pour l'agent {AgentId}.", agentId);
    }

    public async Task<bool> AgentExistsByEmailAsync(string email)
    {
        return await _agentRepository.ExistsByEmailAsync(email);
    }
    
    public async Task<Agent?> GetAgentWithSoldPropertiesAsync(Guid id)
    {
        return await _context.Agents
            .Include(a => a.Properties.Where(p => p.DateSold != null))
            .FirstOrDefaultAsync(a => a.ContactId == id);
    }
    
    public async Task<int> GetTotalSoldPropertiesCountAsync(Guid agentId)
    {
        var agent = await _agentRepository.GetAgentWithSoldPropertiesAsync(agentId);
        return agent?.Properties.Count ?? 0;
    }
}