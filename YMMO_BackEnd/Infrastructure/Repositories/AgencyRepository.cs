using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
{
    public AgencyRepository(YmmoDbContext context) : base(context) { }

    public async Task<Agency?> GetAgencyWithLocationAsync(Guid id)
    {
        return await _dbSet
            .Include(agency => agency.Location)
            .FirstOrDefaultAsync(agency => agency.AgencyId == id);
    }
}