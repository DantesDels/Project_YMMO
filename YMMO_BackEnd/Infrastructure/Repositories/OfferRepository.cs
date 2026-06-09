using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class OfferRepository : BaseRepository<Offer>, IOfferRepository
{
    public OfferRepository(YmmoDbContext context) : base(context) { }
    
    public override async Task<Offer?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(offer => offer.Client)
            .Include(offer => offer.Property)
            .ThenInclude(property => property.Location)
            .FirstOrDefaultAsync(offer => offer.OfferID == id);
    }
    
    public async Task<IEnumerable<Offer>> GetOffersByPropertyAsync(Guid propertyId) {
        return await _dbSet
            .Include(offer => offer.Client)
            .Where(offer => offer.PropertyID == propertyId)
            .Include(offer => offer.Property)
            .ThenInclude(property => property.Location)
            .OrderByDescending(offer => offer.DateCreated)
            .ToListAsync();
    }

    public async Task<IEnumerable<Offer>> GetOffersByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Include(offer => offer.Property)
            .ThenInclude(property => property.Location)
            .Where(offer => offer.ClientID == clientId)
            .OrderByDescending(offer => offer.DateCreated)
            .ToListAsync();
    }
}