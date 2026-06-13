using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Filters;

namespace YMMO.Backend.Domain.Repositories;

public interface IPropertyRepository : IBaseRepository<Property>
{
    // Basic Property management
    Task<IEnumerable<Property>> GetPropertiesByAgencyAsync(Guid agencyId);
    
    Task<IEnumerable<Property>> GetPropertiesByAgentAsync(Guid agentId);

    // Pictures management
    Task AddPictureToPropertyAsync(Guid propertyId, PropertyPicture picture);
    Task UpdatePictureToPropertyAsync(Guid propertyId, PropertyPicture picture);
    Task DeletePictureToPropertyAsync(Guid propertyId, Guid pictureId);
}