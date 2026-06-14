namespace YMMO.Backend.Application.DTOs.Authentification;

public class AuthentificationDto
{
    public record RegisterRequest(string Username, string LastName, string Email, string PhoneNumber, string Password);

    public record LoginRequest(string Email, string Password);

    public record AuthentificationResponse(string Token, string Username, Guid ContactID);

    /// <summary>
    /// Request DTO for the debug agent login endpoint.
    /// Reuses the same fields as LoginRequest for consistency.
    /// The credentials are validated against the Debug section in appsettings,
    /// NOT against the database.
    /// </summary>
    public record DebugLoginRequest(string Email, string Password);
}
