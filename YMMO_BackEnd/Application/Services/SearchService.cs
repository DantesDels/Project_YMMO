using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;
using YMMO.BackEnd.Application.Interfaces;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Application.Services;

public class SearchService : ISearchService
{
    private readonly IYmmoDbContext _context;

    public SearchService(IYmmoDbContext context) => _context = context;
    
    public async Task<IEnumerable<PropertySummaryDto>> SearchAsync(PropertySearchCriteriaDto criteria)
    {
        // Start building the query using IQueryable for deferred execution
        var query = _context.Properties
            .Include(p => p.Location)
            .AsQueryable();

        // Apply filters only if criteria are provided
        if (!string.IsNullOrEmpty(criteria.City))
            query = query.Where(p => p.Location.City == criteria.City);

        if (!string.IsNullOrEmpty(criteria.Region))
            query = query.Where(p => p.Location.Region == criteria.Region);

        if (criteria.Type.HasValue)
            query = query.Where(p => p.PropertyType == criteria.Type.Value);

        // Apply range filters
        if (criteria.MinPrice.HasValue)
            query = query.Where(p => p.CurrentPrice >= criteria.MinPrice.Value);
        
        if (criteria.MaxPrice.HasValue)
            query = query.Where(p => p.CurrentPrice <= criteria.MaxPrice.Value);

        // Filter by required features if provided
        foreach (var feature in criteria.RequiredFeatures)
        {
            query = query.Where(p => p.Features.Contains(feature));
        }

        // Apply pagination to optimize database load
        return await query
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Select(p => new PropertySummaryDto 
            {
                PropertyID = p.PropertyId,
                City = p.Location.City,
                CurrentPrice = p.CurrentPrice
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetSimilarPropertiesAsync(Guid propertyId, int count = 5)
    {
        var sourceProperty = await _context.Properties
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

        if (sourceProperty == null)
            return Enumerable.Empty<PropertySummaryDto>();

        // Query similar properties
        // Logic: Same type, same city, price within +/- 20% range
        return await _context.Properties
            .Where(p => p.PropertyId != propertyId && 
                        p.PropertyType == sourceProperty.PropertyType && 
                        p.Location.City == sourceProperty.Location.City &&
                        p.CurrentPrice >= sourceProperty.CurrentPrice * 0.8m && 
                        p.CurrentPrice <= sourceProperty.CurrentPrice * 1.2m)
            .OrderBy(p => Math.Abs(p.CurrentPrice - sourceProperty.CurrentPrice)) // Sort by price closeness
            .Take(count)
            .Select(p => new PropertySummaryDto
            {
                PropertyID = p.PropertyId,
                City = p.Location.City,
                CurrentPrice = p.CurrentPrice
            })
            .ToListAsync();
    }
}