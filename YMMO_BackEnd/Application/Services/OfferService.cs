using AutoMapper;
using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Application.DTOs.Offers;
using YMMO.Backend.Application.Interfaces;
using YMMO.BackEnd.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Enums;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;
    
    public OfferService(
        IOfferRepository offerRepository, 
        IPropertyRepository propertyRepository, 
        IMapper mapper,
        IUserAccessor userAccessor
        )
    { 
       _offerRepository = offerRepository; 
       _propertyRepository = propertyRepository;
       _mapper = mapper;
       _userAccessor = userAccessor;
    }

    public async Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto dto)
    {
       var property = await _propertyRepository.GetByIdAsync(dto.PropertyID);
       if (property == null || property.DateSold != null)
          throw new InvalidOperationException("Le bien n'est plus disponible.");

       var offer = _mapper.Map<Offer>(dto);
       
       offer.ClientID = _userAccessor.GetCurrentUserId(); 
       offer.OfferID = Guid.NewGuid();
       offer.DateCreated = DateTime.UtcNow;
       offer.StatusOffer = StatusOffer.Pending;
        
       await _offerRepository.AddAsync(offer);
       return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
    }

    public async Task<OfferResponseDto> GetOfferDetailsAsync(Guid offerID, Guid propertyID)
    {
       var offer = await _offerRepository.GetByIdAsync(offerID);
       if (offer == null) throw new KeyNotFoundException("Offre introuvable.");

       return _mapper.Map<OfferResponseDto>(offer);
    }

    public async Task<OfferResponseDto> ReviseOfferPriceAsync(Guid offerID, ReviseOfferPriceDto dto)
    {
       var offer = await _offerRepository.GetByIdAsync(offerID);

       if (offer == null) throw new KeyNotFoundException("Offre introuvable.");
       if (offer.StatusOffer != StatusOffer.Pending)
          throw new InvalidOperationException("Le bien n'est plus disponible.");
       
       _mapper.Map(dto, offer);
       
       await _offerRepository.UpdateAsync(offer);
       return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
    }

    public async Task<OfferResponseDto> UpdateStatusOfferAsync(Guid offerID, UpdateStatusOfferDto dto)
    {
       var offer = await _offerRepository.GetByIdAsync(offerID);

       if (offer == null) throw new KeyNotFoundException("Offre introuvable.");
       if (dto.NewStatus == null) throw new ArgumentNullException(nameof(dto.NewStatus));

       offer.StatusOffer = dto.NewStatus.Value;
       await _offerRepository.UpdateAsync(offer);

       return await GetOfferDetailsAsync(offer.OfferID, offer.PropertyID);
    }
}