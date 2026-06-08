using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Wishlist;

public class RemoveWishlistDto
{
    [Required(ErrorMessage = "L'identifiant du bien est obligatoire.")]
    public Guid? PropertyID { get; set; }
    
    [Required(ErrorMessage = "L'identifiant du client est obligatoire.")]
    public Guid? ClientID { get; set; }
}