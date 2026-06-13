using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Filters;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
{
    public PropertyRepository(YmmoDbContext context) : base(context) {}

    public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync()
    {
        return await _dbSet
            .Where(property => property.DateSold == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetPropertiesByAgencyAsync(Guid agencyId)
    {
        return await _dbSet
            .Where(property => property.AgencyId == agencyId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Property>> GetPropertiesByAgentAsync(Guid agentId)
    {
        return await _dbSet
            .Where(property => property.AgentId == agentId)
            .ToListAsync();
    }

    public async Task AddPictureToPropertyAsync(Guid propertyId, PropertyPicture picture)
    {
        var property = await _context.Properties
            .Include(p => p.Pictures)
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

        if (property == null) throw new KeyNotFoundException("Propriété non trouvée.");

        property.Pictures.Add(picture);
    
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePictureToPropertyAsync(Guid propertyId, PropertyPicture picture)
    {
        var existingPicture = await _context.PropertyPicture
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.PropertyPictureId == picture.PropertyPictureId);

        if (existingPicture == null) throw new KeyNotFoundException("Photo non trouvé.");

        existingPicture.Url = picture.Url;
        existingPicture.DisplayOrder = picture.DisplayOrder;
        existingPicture.IsMain = picture.IsMain;

        await _context.SaveChangesAsync();
    }

    public async Task DeletePictureToPropertyAsync(Guid propertyId, Guid pictureId)
    {
        var pictureToDelete = await _context.PropertyPicture
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.PropertyPictureId == pictureId);

        if (pictureToDelete == null) throw new KeyNotFoundException("Photo non trouvé.");

        _context.PropertyPicture.Remove(pictureToDelete);
        await _context.SaveChangesAsync();
    }
    

    public async Task<IEnumerable<Property>> GetPropertiesByCriteriaAsync(PropertySearchCriteria criteria)
    {
        IQueryable<Property> query = _dbSet.AsQueryable();
        
        // Property Type Filter
        if (criteria.Type.HasValue)
        {
            query = query.Where(property => property.PropertyType == criteria.Type.Value);
        }
        
        // Minimum Price Filter
        if (criteria.MinPrice.HasValue)
        {
            query = query.Where(property => property.CurrentPrice >= criteria.MinPrice.Value);
        }

        // Maximum Price Filter
        if (criteria.MaxPrice.HasValue)
        {
            query = query.Where(property => property.CurrentPrice <= criteria.MaxPrice.Value);
        }

        // Minimum Surface Area Filter
        if (criteria.MinSurface.HasValue)
        {
            query = query.Where(property => property.Surface >= criteria.MinSurface.Value);
        }
        
        // Maximum Surface Area Filter
        if (criteria.MaxSurface.HasValue)
        {
            query = query.Where(property => property.Surface <= criteria.MaxSurface.Value);
        }

        // City Filter (case-insensitive)
        if (!string.IsNullOrWhiteSpace(criteria.City))
        {
            query = query.Include(property => property.Location)
                         .Where(property => property.Location.City.ToLower() == criteria.City.ToLower());
        }

        // Features evaluation (Enum Criteria in the PostgreSQL Features array)
        if (criteria.RequiredCriteria != null && criteria.RequiredCriteria.Any())
        {
            foreach (var req in criteria.RequiredCriteria)
            {
                // EF Core translates 'Contains' on a DB array into an optimized SQL clause
                query = query.Where(property => property.Features.Contains(req));
            }
        }
        return await query.ToListAsync();
    }
}