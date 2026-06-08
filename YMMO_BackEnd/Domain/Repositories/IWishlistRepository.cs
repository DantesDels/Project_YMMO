namespace YMMO.Backend.Domain.Repositories;

public interface IWishlistRepository : IBaseRepository<Wishlist>
{
    Task<IEnumerable<Wishlist>> GetClientWishlistAsync(Guid clientId);
    Task<bool> IsPropertyInWishlistAsync(Guid clientId, Guid propertyId);
}