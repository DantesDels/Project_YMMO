using YMMO.Backend.Application.DTOs.Authentification;

namespace YMMO.Backend.Application.Interfaces;

public interface IAuthentificationService
{
    Task<AuthentificationDto.AuthentificationResponse> LoginAsync(AuthentificationDto.LoginRequest request);
    Task<AuthentificationDto.AuthentificationResponse> RegisterAsync(AuthentificationDto.RegisterRequest request);
    Task LogoutAsync(string token);

    /// <summary>
    /// Debug-only login bypassing the database. Validates against
    /// Debug:AgentEmail / Debug:AgentPassword in appsettings and returns
    /// a JWT with the Agent role. Only works when Debug:EnableAgentAccount
    /// is set to true. Throws UnauthorizedAccessException otherwise.
    /// </summary>
    Task<AuthentificationDto.AuthentificationResponse> DebugLoginAsync(AuthentificationDto.DebugLoginRequest request);
}