namespace YMMO.Backend.Application.Interfaces;

public interface IAuthService
{
    // Returns a token (JWT) if authentication succeeds
    Task<string> LoginAsync(string email, string password);
    
    // Registers a new user
    Task<bool> RegisterAsync(string email, string password);
    
    // Revokes a token or handles logout
    Task LogoutAsync(string token);
}