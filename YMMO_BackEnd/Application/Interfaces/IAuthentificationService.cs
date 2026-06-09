using YMMO.Backend.Application.DTOs.Authentification;

namespace YMMO.Backend.Application.Interfaces;

public interface IAuthentificationService
{
    Task<AuthentificationDto.AuthentificationResponse> LoginAsync(AuthentificationDto.LoginRequest request);
    Task<AuthentificationDto.AuthentificationResponse> RegisterAsync(AuthentificationDto.RegisterRequest request);
    Task LogoutAsync(string token);
}