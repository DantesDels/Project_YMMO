using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Application.DTOs.Offers;
using YMMO.BackEnd.Application.Interfaces;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Enums;

namespace YMMO.Backend.Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IPropertyRepository _propertyRepository;
    
    public OfferService(IOfferRepository offerRepository, IPropertyRepository propertyRepository) 
    { 
        _offerRepository = offerRepository; 
        _propertyRepository = propertyRepository;
    }

    public async Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto dto)
    {
        var property = await _propertyRepository.GetByIdAsync(dto.PropertyID);
        
        if (property == null || property.DateSold != null)
            throw new InvalidOperationException("Le bien n'est plus disponible.");

        var offer = new Offer
        {
            OfferID = Guid.NewGuid(),
            DateCreated =  DateTime.UtcNow,
            PropertyID = dto.PropertyID,
            ClientID = dto.ClientID,
            OfferPrice = dto.OfferPrice,
            StatusOffer = StatusOffer.Pending
        };
        
        await _offerRepository.AddAsync(offer);
        return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
    }

	public async Task<OfferResponseDto> ReviseOfferPriceAsync(Guid offerID, ReviseOfferPriceDto dto)
	{
		var offer = await _offerRepository.GetByIdAsync(offerID);

		if (offer == null)
			throw new KeyNotFoundException("Offre introuvable.");
		else if (offer.StatusOffer != StatusOffer.Pending)
			throw new InvalidOperationException("Le bien n'est plus disponible.");
		
		offer.OfferPrice = dto.NewOfferPrice;
		await _offerRepository.UpdateAsync(offer);
		
		return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
	}

	public async Task<OfferResponseDto> UpdateStatusOfferAsync(Guid offerID, UpdateStatusOfferDto dto)
	{
		var offer = await _offerRepository.GetByIdAsync(offerID);

		if (offer == null)
			throw new KeyNotFoundException("Offre introuvable.");
		else if (dto.NewStatus == null)
			throw new ArgumentNullException(nameof(dto.NewStatus));

		offer.StatusOffer = dto.NewStatus.Value;
		await _offerRepository.UpdateAsync(offer);

		return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
	}

	public async Task<OfferResponseDto> GetOfferDetailsAsync(Guid offerID,  Guid propertyID)
	{
		var offer = await _offerRepository.GetByIdAsync(offerID);

		if (offer == null)
			throw new KeyNotFoundException("Offre introuvable.");

		return new OfferResponseDto
		{
		OfferID = offer.OfferID,
		PropertyID = offer.PropertyID,
		ClientID = offer.ClientID,
		ClientLastName = offer.Client?.LastName ?? "Inconnu",
        ClientFirstName = offer.Client?.FirstName ?? "Inconnu",
		ClientPhoneNumber = offer.Client?.PhoneNumber ?? "Inconnu",
		OfferPrice = offer.OfferPrice,
		StatusOffer = offer.StatusOffer.ToString(),
		PropertyCity = offer.Property?.Location?.City ?? "Inconnu",
		PropertyRegion = offer.Property?.Location?.Region ?? "Inconnu"
		};
	}
}