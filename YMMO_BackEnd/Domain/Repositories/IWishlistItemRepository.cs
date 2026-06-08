using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IWishlistItemRepository : IBaseRepository<WishlistItem>
{
    Task<IEnumerable<WishlistItem>> GetClientWishlistItemsAsync(Guid clientId);
    
    // Check if a specific property is already in the client's wishlist
    Task<bool> IsPropertyInWishlistAsync(Guid clientId, Guid propertyId);

    // Retrieve a specific favorite record using composite keys for deletion
    Task<WishlistItem?> GetByIdsAsync(Guid clientId, Guid propertyId);
}