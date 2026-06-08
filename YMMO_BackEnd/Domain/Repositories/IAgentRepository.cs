using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IAgentRepository : IBaseRepository<Agent>
{
    Task<Agent?> GetAgentWithSoldPropertiesAsync(Guid id);
}