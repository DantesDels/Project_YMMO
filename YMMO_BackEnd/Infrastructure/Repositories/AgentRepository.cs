using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class AgentRepository : BaseRepository<Agent>, IAgentRepository
{
    public AgentRepository(YmmoDbContext context) : base(context) {}

    public async Task<Agent?> GetAgentWithSoldPropertiesAsync(Guid id)
    {
        return await _context.Agents
            .Include(a => a.Properties.Where(p => p.DateSold != null))
            .FirstOrDefaultAsync(a => a.ContactId == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Agents.AnyAsync(a => a.Email == email);
    }

    public async Task<int> GetTotalSoldPropertiesCountAsync(Guid agentId)
    {
        // On utilise CountAsync sur la requête pour ne pas charger les propriétés en mémoire
        return await _context.Properties
            .CountAsync(p => p.AgentID == agentId && p.DateSold != null);
    }
}