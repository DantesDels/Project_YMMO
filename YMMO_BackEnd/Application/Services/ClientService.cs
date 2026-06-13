using AutoMapper;
using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Application.DTOs.Wishlist;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;


namespace YMMO.Backend.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IWishlistItemRepository _wishlistItemRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    
    public ClientService(
        IClientRepository clientRepository, 
        IWishlistItemRepository wishlistItemRepository, 
        IPasswordHasher passwordHasher,
        IUserAccessor userAccessor,
        IMapper mapper) 
    {
        _clientRepository = clientRepository;
        _wishlistItemRepository = wishlistItemRepository;
        _passwordHasher = passwordHasher;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ClientProfileDto?> GetProfileAsync(Guid clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        return client == null ? null : _mapper.Map<ClientProfileDto>(client);
    }

    public async Task<ClientProfileDto> RegisterClientAsync(RegisterClientDto dto)
    {
        if (await _clientRepository.GetByEmailAsync(dto.Email) != null) 
            throw new ArgumentException("Cet email existe déjà.");

        var newClient = _mapper.Map<Client>(dto);
        newClient.CreatedAt = DateTime.UtcNow;
        newClient.PasswordHash = _passwordHasher.Hash(dto.Password);

        await _clientRepository.AddAsync(newClient);
        
        return _mapper.Map<ClientProfileDto>(newClient);
    }

    public async Task<ClientProfileDto> UpdateProfileAsync(Guid clientId, UpdateClientDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) throw new KeyNotFoundException("Client introuvable.");

        _mapper.Map(dto, client);

        await _clientRepository.UpdateAsync(client);
        return _mapper.Map<ClientProfileDto>(client);
    }

    // --- SECTION WISHLIST ---

    public async Task AddToWishlistAsync(AddWishlistDto dto)
    {
        var clientId = _userAccessor.GetCurrentUserId();
        
        if (!dto.PropertyID.HasValue)
        {
            throw new ArgumentException("PropertyID est obligatoire.");
        }
        
        var wishlistItem = new WishlistItem
        {
            ClientID = clientId, // Secure the Token
            PropertyID = dto.PropertyID.Value,
        };
        
        await _wishlistItemRepository.AddAsync(wishlistItem);
    }

    public async Task RemoveFromWishlistAsync(RemoveWishlistDto dto)
    {
        var clientId = _userAccessor.GetCurrentUserId();
        
        if (!dto.PropertyID.HasValue)
        {
            throw new ArgumentException("PropertyID est obligatoire.");
        }
        
        // Retrieve the client identity from the authenticated session (JWT) instead of relying on the DTO. 
        // This ensures that a client can only modify their own wishlist, preventing ID spoofing 
        // and unauthorized access to other users' data.
        var wishlistItem = await _wishlistItemRepository.GetByIdsAsync(clientId, dto.PropertyID.Value);
        
        if (wishlistItem != null)
        {
            await _wishlistItemRepository.DeleteAsync(wishlistItem);
        }
    }
}