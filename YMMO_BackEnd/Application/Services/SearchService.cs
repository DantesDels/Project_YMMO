using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;

namespace YMMO.Backend.Application.Services;

public class SearchService : ISearchService
{
    private readonly IYmmoDbContext _context;
    private readonly IMapper _mapper;

    public SearchService(IYmmoDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<PropertySummaryDto>> SearchAsync(PropertySearchCriteriaDto criteria)
    {
        var query = _context.Properties
            .Include(p => p.Location)
            .AsQueryable();

        // Dynamic filtering
        if (!string.IsNullOrEmpty(criteria.City))
            query = query.Where(p => p.Location.City.ToLower() == criteria.City.ToLower());

        if (!string.IsNullOrEmpty(criteria.Region))
            query = query.Where(p => p.Location.Region.ToLower() == criteria.Region.ToLower());

        if (criteria.Type.HasValue)
            query = query.Where(p => p.PropertyType == criteria.Type.Value);

        if (criteria.MinPrice.HasValue)
            query = query.Where(p => p.CurrentPrice >= criteria.MinPrice.Value);
        
        if (criteria.MaxPrice.HasValue)
            query = query.Where(p => p.CurrentPrice <= criteria.MaxPrice.Value);

        if (criteria.Conditions.Count != 0)
            query = query.Where(p => criteria.Conditions.Contains(p.Condition));

        if (criteria.EnergyClasses.Count != 0)
            query = query.Where(p => criteria.EnergyClasses.Contains(p.EnergyClass));

        // Filter by features (PostgreSQL text[] containment)
        foreach (var feature in criteria.RequiredCriteria)
        {
            query = query.Where(p => p.Features.Contains(feature));
        }

        // Projecting to DTO using AutoMapper's ProjectTo for better performance
        return await query
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ProjectTo<PropertySummaryDto>(_mapper.ConfigurationProvider) 
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetSimilarPropertiesAsync(Guid propertyId, int count = 5)
    {
        var sourceProperty = await _context.Properties
            .Include(p => p.Location)
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

        if (sourceProperty == null)
            return Enumerable.Empty<PropertySummaryDto>();

        return await _context.Properties
            .Where(p => p.PropertyId != propertyId && 
                        p.PropertyType == sourceProperty.PropertyType && 
                        p.Location.City == sourceProperty.Location.City &&
                        p.CurrentPrice >= sourceProperty.CurrentPrice * 0.8m && 
                        p.CurrentPrice <= sourceProperty.CurrentPrice * 1.2m)
            .OrderBy(p => Math.Abs(p.CurrentPrice - sourceProperty.CurrentPrice))
            .Take(count)
            .ProjectTo<PropertySummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}