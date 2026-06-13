namespace YMMO.Backend.Application.DTOs.Authentification;

public class AuthentificationDto
{
    public record RegisterRequest(string Username, string LastName, string Email, string PhoneNumber, string Password);

    public record LoginRequest(string Email, string Password);

    public record AuthentificationResponse(string Token, string Username, Guid ContactID);
}
