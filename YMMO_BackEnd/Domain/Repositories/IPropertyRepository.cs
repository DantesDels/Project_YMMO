using YMMO.Backend.Domain.Entites;
using YMMO.Backend.Domain.Filters;

namespace YMMO.Backend.Domain.Repositories;

public interface IPropertyRepository : IBaseRepository<Property>
{
    Task<IEnumerable<Property>> GetAvailablePropertiesAsync();
    Task<IEnumerable<Property>> GetPropertiesByAgencyAsync(Guid agencyId);
    Task<IEnumerable<Property>> GetPropertiesByCriteriaAsync(PropertySearchCriteria criteria);