using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Authentification;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Interfaces;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthentificationController : ControllerBase
{
    private readonly IAuthentificationService _authentificationService;

    public AuthentificationController(IAuthentificationService authentificationService)
    {
        _authentificationService = authentificationService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthentificationDto.AuthentificationResponse), 201)]
    public async Task<IActionResult> RegisterAsync([FromBody] AuthentificationDto.RegisterRequest request) 
    {
        var response = await _authentificationService.RegisterAsync(request);
        return StatusCode(201, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthentificationDto.LoginRequest request)
    {
        // The service handles everything: validation, hashing, and token generation
        var response = await _authentificationService.LoginAsync(request);
    
        return Ok(response); 
    }
    
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Avec le système LocalStorage, la déconnexion se gère à 100% côté Javascript
        // en supprimant le token du navigateur. L'API a juste besoin de dire OK.
        return Ok(new { message = "Déconnexion réussie." });
    }

    /// <summary>
    /// Debug-only agent login endpoint.
    ///
    /// Bypasses the database and validates credentials against the Debug section
    /// in appsettings. Returns a valid JWT with the Agent role when the
    /// Debug:EnableAgentAccount flag is set to true.
    ///
    /// Disabled by default in production. Hidden from Swagger documentation
    /// via [ApiExplorerSettings(IgnoreApi = true)].
    ///
    /// Usage (development only):
    ///   POST /api/authentification/debug-login
    ///   { "email": "agent@debug.ymmo", "password": "Debug@Agent1" }
    ///
    /// See AuthentificationService.DebugLoginAsync for implementation details.
    /// </summary>
    [HttpPost("debug-login")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> DebugLoginAsync([FromBody] AuthentificationDto.DebugLoginRequest request)
    {
        var response = await _authentificationService.DebugLoginAsync(request);
        return Ok(response);
    }
}