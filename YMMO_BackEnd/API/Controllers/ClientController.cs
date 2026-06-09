using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Client;
using YMMO.Backend.Application.DTOs.Wishlist;
using YMMO.Backend.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    // Register a new client (Public)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterClientDto dto)
    {
        var client = await _clientService.RegisterClientAsync(dto);
        return Ok(client);
    }

    // Get client profile
    [Authorize(Roles = "Client,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var profile = await _clientService.GetProfileAsync(id);
        return profile != null ? Ok(profile) : NotFound();
    }

    // Update client profile
    [Authorize(Roles = "Client,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateClientDto dto)
    {
        var updatedProfile = await _clientService.UpdateProfileAsync(id, dto);
        return Ok(updatedProfile);
    }

    // Add property to wishlist
    [Authorize(Roles = "Client")]
    [HttpPost("wishlist")]
    public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
    {
        await _clientService.AddToWishlistAsync(dto);
        return NoContent();
    }

    // Remove property from wishlist
    [Authorize(Roles = "Client")]
    [HttpDelete("wishlist")]
    public async Task<IActionResult> RemoveFromWishlist([FromBody] RemoveWishlistDto dto)
    {
        await _clientService.RemoveFromWishlistAsync(dto);
        return NoContent();
    }
}