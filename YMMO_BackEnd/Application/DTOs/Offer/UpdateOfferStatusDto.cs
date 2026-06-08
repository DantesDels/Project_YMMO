// DTOs/Offers/UpdateOfferStatusDto.cs
using System.ComponentModel.DataAnnotations;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.DTOs.Offers;

public class UpdateOfferStatusDto
{
    [Required(ErrorMessage = "Le nouveau statut est obligatoire.")]
    public StatusOffer? NewStatus { get; set; }
}