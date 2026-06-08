using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Application.DTOs.Wishlist;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IWishlistItemRepository _wishlistItemRepository;
    
    public ClientService(IClientRepository clientRepository, IWishlistItemRepository wishlistItemRepository)
    {
        _clientRepository = clientRepository;
        _wishlistItemRepository = wishlistItemRepository;
    }

    public async Task<ClientProfileDto?> GetProfileAsync(Guid clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) return null;

        return new ClientProfileDto
        {
            ContactId = client.ContactID,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            
            // Counting items for Dashboard Vue.js
            ActiveOffersCount = client.Offers?.Count ?? 0,
            WishlistItemsCount = client.WishlistItems?.Count ?? 0
        };
    }

    public async Task<Guid> RegisterClientAsync(RegisterClientDto dto)
    {
        var existingClient = await _clientRepository.GetByEmailAsync(dto.Email);
        if(existingClient != null) throw new Exception("Cet email existe déjà.");

        var newClient = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            // Le mot de passe (dto.Password) devra être haché ici plus tard avec BCrypt !
        };

        await _clientRepository.AddAsync(newClient);
        return newClient.ContactID;
    }

    public async Task UpdateProfileAsync(Guid clientId, UpdateClientDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) throw new KeyNotFoundException("Client introuvable.");

        client.FirstName = dto.FirstName;
        client.LastName = dto.LastName;
        client.Email = dto.Email;
        client.PhoneNumber = dto.PhoneNumber;
        
        // If Agent is being assigned to Client
        if (dto.AgentID.HasValue)
        {
            client.AgentID = dto.AgentID.Value;
        }

        await _clientRepository.UpdateAsync(client);
    }

    // --- SECTION WISHLIST ---

    public async Task AddToWishlistAsync(AddWishlistDto dto)
    {
        var wishlistItem = new WishlistItem
        {
            ClientID = dto.ClientID.Value,
            PropertyID = dto.PropertyID.Value,
        };
        
        await _wishlistItemRepository.AddAsync(wishlistItem);
    }

    public async Task RemoveFromWishlistAsync(RemoveWishlistDto dto)
    {
        // Use composite key (ClientID, PropertyID) to identify the unique favorite relationship.
        var wishlistItem = await _wishlistItemRepository.GetByIdsAsync(dto.ClientID.Value, dto.PropertyID.Value);
        
        if (wishlistItem != null)
        {
            await _wishlistItemRepository.DeleteAsync(wishlistItem);
        }
    }
}