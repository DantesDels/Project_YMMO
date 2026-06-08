// DTOs/Offers/ReviseOfferPriceDto.cs
using System.ComponentModel.DataAnnotations;

namespace YMMO.Backend.Application.DTOs.Offers;

public class ReviseOfferPriceDto
{
    [Required(ErrorMessage = "Le nouveau prix est obligatoire.")]
    [Range(1, 100_000_000, ErrorMessage = "Le prix doit être supérieur à 0.")]
    public decimal? NewOfferPrice { get; set; }
}