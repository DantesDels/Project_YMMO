using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Application.DTOs.Location;

namespace YMMO.Backend.Application.DTOs.Agency;

public class CreateAgencyDto
{
    [Required(ErrorMessage = "Le nom de l'agence est obligatoire.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Format d'email invalide.")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public CreateLocationDto Location { get; set; } = null!;
}