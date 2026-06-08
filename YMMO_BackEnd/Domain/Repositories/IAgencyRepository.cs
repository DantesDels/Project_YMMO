using System;
using System.Threading.Tasks;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IAgencyRepository : IBaseRepository<Agency>
{
    Task<Agency?> GetAgencyWithLocationAsync(Guid id);
}