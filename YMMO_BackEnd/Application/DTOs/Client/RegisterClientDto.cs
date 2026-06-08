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
    public string Password { get; set; } = string.Empty; 
}