namespace YMMO.Backend.Application.DTOs.Authentification;

public class RegisterResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}