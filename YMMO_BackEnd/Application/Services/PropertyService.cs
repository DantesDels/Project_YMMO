using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Application.DTOs.Location;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;

namespace YMMO.Backend.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetCatalogAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();

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
    }

    public async Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null) return null;

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
    }

    public async Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto)
    {
        var property = new Property
        {
            PropertyType = dto.PropertyType,
            YearBuilt =  dto.YearBuilt,
            Condition = dto.Condition,
            EnergyClass = dto.EnergyClass,
            InitialPrice = dto.InitialPrice,
            CurrentPrice = dto.InitialPrice, // Defaults to initial price on creation
            Surface = dto.Surface,
            Features = dto.Features,
            DateListed = DateTime.UtcNow,
            
            AgencyID = dto.AgencyID,
            AgentID = dto.AgentID,
            SellerID = dto.SellerID,

            Location = new Location
            {
                Address = dto.Location.Address,
                City = dto.Location.City,
                Region = dto.Location.Region,
                PostalCode = dto.Location.PostalCode,
                Country = dto.Location.Country
            }
        };

        await _propertyRepository.AddAsync(property);

        return new PropertyDetailDto();
    }

    public async Task<PropertyDetailDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        
        if (property == null)
        {
            throw new KeyNotFoundException($"Property with ID {id} was not found.");
        }

        property.CurrentPrice = dto.CurrentPrice;
        property.Condition = dto.Condition;
        property.EnergyClass = dto.EnergyClass;
        property.Features = dto.Features;

        await _propertyRepository.UpdateAsync(property);
        
        return new PropertyDetailDto();
    }

    public async Task<bool> DeletePropertyAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        
        if (property != null)
        {
            await _propertyRepository.DeleteAsync(property);
        }   
        return true;
    }
}