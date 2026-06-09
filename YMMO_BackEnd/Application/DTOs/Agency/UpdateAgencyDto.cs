using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Application.DTOs.Location;

namespace YMMO.Backend.Application.DTOs.Agency;

public class UpdateAgencyDto
{
    [Required(ErrorMessage = "Le nom de l'agence est obligatoire.")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Format de numéro de téléphone invalide.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required] 
    public LocationDto NewAddress { get; set; } = null!;
}