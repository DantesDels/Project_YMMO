using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Authentification;

public class UpdatePasswordDto
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Le mot de passe doit faire au moins 8 caractères.")]
    public string NewPassword { get; set; } = string.Empty;
}