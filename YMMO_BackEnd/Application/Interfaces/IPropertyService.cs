using YMMO.Backend.Application.DTOs.Property;

namespace YMMO.Backend.Application.Interfaces;

public interface IPropertyService
{
    // Read
    Task<IEnumerable<PropertySummaryDto>> GetCatalogAsync();
    Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id);

    // Write
    Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto);
    Task<PropertyDetailDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto dto);
    
    // Deletion
    Task<bool> DeletePropertyAsync(Guid id);
}