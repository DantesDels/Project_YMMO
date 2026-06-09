// DTOs/Clients/RegisterClientDto.cs
using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Client;

public class RegisterClientDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    // format E.164 needed
    [RegularExpression(@"^\+[1-9]\d{1,14}$", ErrorMessage = "Le numéro doit être au format international (+33...)")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Managed with JWT
    [Required]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial.")]
    public string Password { get; set; } = string.Empty;
}