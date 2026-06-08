using System;
using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Agent;

public class UpdateAgentDto
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

    // if the Agent moves in a different Agency
    [Required]
    public Guid? AgencyID { get; set; } 
}