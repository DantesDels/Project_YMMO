using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Application.DTOs.Location;

namespace YMMO.Backend.Application.DTOs.Agency;

public class UpdateAgencyDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required] 
    public CreateLocationDto NewAddress { get; set; } = null!;
}