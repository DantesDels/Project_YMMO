using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Location;

public class CreateLocationDto
{
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string PostalCode { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
}