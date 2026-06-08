using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Application.DTOs.Wishlist;

namespace YMMO.Backend.Application.Interfaces;

public interface IClientService
{
    Task<ClientProfileDto?> GetProfileAsync(Guid clientId);
    Task UpdateProfileAsync(Guid clientId, UpdateClientDto dto);
    
    Task<Guid> RegisterClientAsync(RegisterClientDto dto);
    
    Task AddToWishlistAsync(AddWishlistDto dto);
    Task RemoveFromWishlistAsync(RemoveWishlistDto dto);
}