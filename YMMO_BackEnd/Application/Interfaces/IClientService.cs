using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Application.DTOs.Wishlist;

namespace YMMO.Backend.Application.Interfaces;

public interface IClientService
{
    Task<ClientProfileDto?> GetProfileAsync(Guid clientId);
    Task<ClientProfileDto> UpdateProfileAsync(Guid clientId, UpdateClientDto dto);
    
    Task<ClientProfileDto> RegisterClientAsync(RegisterClientDto dto);
    
    Task AddToWishlistAsync(AddWishlistDto dto);
    Task RemoveFromWishlistAsync(RemoveWishlistDto dto);
}