using System;
using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Client;

public class UpdateClientDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    // Format E.164
    [RegularExpression(@"^\+[1-9]\d{1,14}$", ErrorMessage = "Format international requis (+33...)")]
    public string PhoneNumber { get; set; } = string.Empty;

    // Nullable while without LinkedAgent 
    public Guid? AgentID { get; set; } 
}