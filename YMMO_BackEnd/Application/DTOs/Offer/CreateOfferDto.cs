using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Offer;

public class CreateOfferDto
{
    [Required(ErrorMessage = "Le bien immobilier est obligatoire.")]
    public Guid PropertyID { get; set; } // Which property ?
    
    [Required(ErrorMessage = "L'acheteur est obligatoire.")]
    public Guid ClientID { get; set; } // Who's buying ?
    
    [Required(ErrorMessage = "Le prix proposé est obligatoire.")]
    [Range(1, 100_000_000, ErrorMessage = "Le prix doit être supérieur à 0.")]
    public decimal OfferPrice { get; set; } // What's his offer ?
}