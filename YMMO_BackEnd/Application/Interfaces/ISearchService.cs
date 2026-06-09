using YMMO.Backend.Application.DTOs.Property;

namespace YMMO.Backend.Application.Interfaces;

public interface ISearchService
{
    // Executes a search based on complex criteria provided by the client
    Task<IEnumerable<PropertySummaryDto>> SearchAsync(PropertySearchCriteriaDto criteria);
    
    // Finds similar properties based on a base property ID for recommendations
    Task<IEnumerable<PropertySummaryDto>> GetSimilarPropertiesAsync(Guid propertyId, int count = 5);
}