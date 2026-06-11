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
        
        // Use AutoMapper to project entities to DTOs
        return _mapper.Map<IEnumerable<PropertySummaryDto>>(properties);
    }

   public async Task<PropertyDetailDto?> GetPropertyByIdAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null) return null;

        // Use AutoMapper for detailed view mapping
        return _mapper.Map<PropertyDetailDto>(property);
    }
    
    public async Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto)
    {
        // AutoMapper creates the 'Property' object and automatically copies 
        // all matching properties.
        var property = _mapper.Map<Property>(dto);
    
        // Apply business logic specific to server-side context
        property.DateListed = DateTime.UtcNow;
        property.CurrentPrice = dto.InitialPrice;
    
        // Persist the entity
        await _propertyRepository.AddAsync(property);
    
        // Return the mapped DTO
        return _mapper.Map<PropertyDetailDto>(property);
    }
    
    public async Task<IEnumerable<PropertySummaryDto>> GetAgencyPropertiesAsync()
    {
        // Retrieve agency context from the authenticated user
        var agencyId = _userAccessor.GetCurrentAgencyId();
    
        // Delegate filtering by agency to the repository
        var properties = await _propertyRepository.GetPropertiesByAgencyAsync(agencyId);
    
        return _mapper.Map<IEnumerable<PropertySummaryDto>>(properties);
    }
    
    public async Task<PropertyDetailDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null) throw new KeyNotFoundException($"La propriété ID {id} n'a pas été trouvé.");
    
        // AutoMapper detects identical fields between DTO and Entity 
        // to update existing instance.
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
    
    public async Task AddPictureToPropertyAsync(Guid propertyId, PropertyPicture picture)
    {
        picture.PropertyId = propertyId;
        await _propertyRepository.AddPictureToPropertyAsync(propertyId, picture);
    }

    public async Task UpdatePictureToPropertyAsync(Guid propertyId, PropertyPicture picture)
    {
        await _propertyRepository.UpdatePictureToPropertyAsync(propertyId, picture);
    }

    public async Task DeletePictureToPropertyAsync(Guid propertyId, Guid pictureId)
    {
        await _propertyRepository.DeletePictureToPropertyAsync(propertyId, pictureId);
    }
}