using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class AgentRepository : BaseRepository<Agent>, IAgentRepository
{
    public AgentRepository(YmmoDbContext context) : base(context) { }

    public async Task<Agent?> GetAgentWithSoldPropertiesAsync(Guid id)
    {
        return await _dbSet
            .Include(agent => agent.SoldProperties)
            .FirstOrDefaultAsync(agent => agent.ContactId == id);
    }
}