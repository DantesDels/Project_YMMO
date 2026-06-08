using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Wishlist;

public class AddWishlistDto
{
    [Required]
    public Guid? PropertyID { get; set; }
    
    [Required]
    public Guid? ClientID { get; set; }
}