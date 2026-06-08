using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class WishlistRepository : BaseRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(YmmoDbContext context) : base(context) { }

    public async Task<IEnumerable<Wishlist>> GetClientWishlistAsync(Guid clientId)
    {
        return await _dbSet
            .Include(wishlist => wishlist.Property)
            .Where(wishlist => wishlist.ClientID == clientId)
            .ToListAsync();
    }

    public async Task<bool> IsPropertyInWishlistAsync(Guid clientId, Guid propertyId)
    {
        return await _dbSet
            .AnyAsync(wishlist => wishlist.ClientID == clientId && wishlist.PropertyID == propertyId);
    }
}