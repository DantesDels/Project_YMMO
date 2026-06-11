using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Application.Interfaces;

public interface IPropertyService
{
    // Read operations: Public access
    Task<IEnumerable<PropertySummaryDto>> GetAllPropertiesAsync();
    Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id);

    // Write operations: Authorized access only
    Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto);
    Task<PropertyDetailDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto dto);
    
    // Deletion: Authorized access only
    Task<bool> DeletePropertyAsync(Guid id);
    
    // Property Pictures management
    Task AddPictureToPropertyAsync(Guid propertyId, PropertyPicture picture);
    Task UpdatePictureToPropertyAsync(Guid propertyId, PropertyPicture picture);
    Task DeletePictureToPropertyAsync(Guid propertyId, Guid pictureId);
}