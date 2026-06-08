using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class WishlistItemRepository : BaseRepository<WishlistItem>, IWishlistItemRepository
{
    public WishlistItemRepository(YmmoDbContext context) : base(context) {}
    
    public async Task<IEnumerable<WishlistItem>> GetClientWishlistItemsAsync(Guid clientId)
    {
        return await _dbSet
            .Include(w => w.Property)
            .Where(w => w.ClientID == clientId)
            .ToListAsync();
    }

    public async Task<WishlistItem?> GetByIdsAsync(Guid clientId, Guid propertyId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.ClientID == clientId && w.PropertyID == propertyId);
    }

    public async Task<bool> IsPropertyInWishlistAsync(Guid clientId, Guid propertyId)
    {
        return await _dbSet
            .AnyAsync(w => w.ClientID == clientId && w.PropertyID == propertyId);
    }
}