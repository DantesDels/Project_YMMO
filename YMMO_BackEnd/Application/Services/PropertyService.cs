using AutoMapper;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;
    private readonly IUserAccessor _userAccessor;

    public PropertyService(
        IPropertyRepository propertyRepository, 
        IMapper mapper,
        IUserAccessor userAccessor
        )
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
        _userAccessor = userAccessor;
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetAllPropertiesAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();
        
        // BEFORE MAPPER 
        /*
        return properties.Select(p => new PropertySummaryDto
        {
            PropertyID = p.PropertyID,
            PropertyType = p.PropertyType.ToString(),
            Condition = p.Condition.ToString(),
            CurrentPrice = p.CurrentPrice,
            Surface = p.Surface,
        
            // Safe navigation for flattened data
            City = p.Location?.City ?? string.Empty,
            PostalCode = p.Location?.PostalCode ?? string.Empty,
        
            // Limit features for the summary view
            MainFeatures = p.Features.Take(3).Select(f => f.ToString()).ToList()
        }).ToList();
        */

        //AFTER MAPPER
        return _mapper.Map<IEnumerable<PropertySummaryDto>>(properties);
    }

   public async Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null) return null;

        // BEFORE MAPPER
        /*
        return new PropertyDetailDto
        {
            PropertyID = property.PropertyID,
            YearBuilt = property.YearBuilt,
            PropertyType = property.PropertyType.ToString(),
            Condition = property.Condition.ToString(),
            EnergyClass = property.EnergyClass.ToString(),
            CurrentPrice = property.CurrentPrice,
            InitialPrice = property.InitialPrice,
            Surface = property.Surface,
            DateListed = property.DateListed,
            Features = property.Features.Select(f => f.ToString()).ToList(),

            Location = new LocationDto
            {
                Address = property.Location?.Address ?? string.Empty,
                City = property.Location?.City ?? string.Empty,
                PostalCode = property.Location?.PostalCode ?? string.Empty,
                Country = property.Location?.Country ?? string.Empty
            },

            Agent = new AgentContactDto
            {
                FirstName = property.Agent?.FirstName ?? string.Empty,
                LastName = property.Agent?.LastName ?? string.Empty,
                Email = property.Agent?.Email ?? string.Empty,
                PhoneNumber = property.Agent?.PhoneNumber ?? string.Empty
            }
        };
        */
    
        // AFTER MAPPER
        return _mapper.Map<PropertyDetailDto>(property);
    }
    
    public async Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto)
    {
        // BEFORE MAPPER
        /*
        var property = new Property
        {
            PropertyType = dto.PropertyType,
            YearBuilt =  dto.YearBuilt,
            Condition = dto.Condition,
            EnergyClass = dto.EnergyClass,
            InitialPrice = dto.InitialPrice,
            CurrentPrice = dto.InitialPrice, 
            Surface = dto.Surface,
            Features = dto.Features,
            DateListed = DateTime.UtcNow,
            
            AgencyID = dto.AgencyID,
            AgentID = dto.AgentID,
            SellerID = dto.SellerID,
        
            Location = new Location // Mapping manuel laborieux
            {
                Address = dto.Location.Address,
                City = dto.Location.City,
                Region = dto.Location.Region,
                PostalCode = dto.Location.PostalCode,
                Country = dto.Location.Country
            }
        };
        */
        
        // NOUVELLE VERSION (AUTOMAPPER)
        // AutoMapper crée l'objet 'Property' et y copie automatiquement 
        // toutes les propriétés dont les noms correspondent.
            var property = _mapper.Map<Property>(dto);
    
        // Tu ne rajoutes que ce qui est spécifique à la logique métier
        // (ce qui n'est pas dans le DTO ou qui dépend du contexte serveur)
            property.DateListed = DateTime.UtcNow;
            property.CurrentPrice = dto.InitialPrice;
    
        // Persistance
            await _propertyRepository.AddAsync(property);
    
        // Conversion automatique de l'entité vers le DTO de réponse
            return _mapper.Map<PropertyDetailDto>(property);
    }
    
    public async Task<IEnumerable<PropertySummaryDto>> GetAgencyPropertiesAsync()
    {
        // On récupère l'agence directement depuis le contexte utilisateur
        var agencyId = _userAccessor.GetCurrentAgencyId();
    
        // On délègue au repository le filtrage par agence
        var properties = await _propertyRepository.GetPropertiesByAgencyAsync(agencyId);
    
        return _mapper.Map<IEnumerable<PropertySummaryDto>>(properties);
    }
    
    public async Task<PropertyDetailDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null) throw new KeyNotFoundException($"Le bien ID {id} n'a pas été trouvé.");
    
        // AutoMapper détecte les champs identiques entre UpdatePropertyDto et Property
        // et met à jour uniquement ceux-ci.
        _mapper.Map(dto, property);
    
        await _propertyRepository.UpdateAsync(property);
        
        return _mapper.Map<PropertyDetailDto>(property);
    }
    
    public async Task<bool> DeletePropertyAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
    
        if (property == null)
            return false;

        await _propertyRepository.DeleteAsync(property);
    
        return true;
    }
}