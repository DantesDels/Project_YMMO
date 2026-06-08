using YMMO.Backend.Application.DTOs.Property;

namespace YMMO.Backend.Application.Interfaces;

public interface IPropertyService
{
    // Read
    Task<IEnumerable<PropertySummaryDto>> GetCatalogAsync();
    Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id);

    // Write
    Task<Guid> CreatePropertyAsync(CreatePropertyDto dto);
    Task UpdatePropertyAsync(Guid id, UpdatePropertyDto dto);
    Task DeletePropertyAsync(Guid id);
}