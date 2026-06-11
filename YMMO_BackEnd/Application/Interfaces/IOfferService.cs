using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Application.DTOs.Offers; 

namespace YMMO.Backend.Application.Interfaces;

public interface IOfferService
{ 
    Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto dto);
    Task<OfferResponseDto> GetOfferDetailsAsync(Guid offerId,  Guid propertyId);
    Task<OfferResponseDto> ReviseOfferPriceAsync(Guid offerId, ReviseOfferPriceDto dto);
    Task<OfferResponseDto> UpdateStatusOfferAsync(Guid offerId, UpdateStatusOfferDto dto);
}