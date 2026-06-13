using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.DTOs.PropertyPicture;
using YMMO.Backend.Application.Interfaces;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IMapper _mapper;

    public PropertyController(IPropertyService propertyService, IMapper mapper)
    {
        _propertyService = propertyService;
        _mapper = mapper;
    }

    // Publicly accessible: everyone can search and view property listings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _propertyService.GetAllPropertiesAsync();
        return Ok(properties);
    }

    // Publicly accessible
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await _propertyService.GetPropertyByIdAsync(id);
        return Ok(property);
    }

    // Restricted: Only Agents, Managers, and Admins can create new listings
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        var property = await _propertyService.CreatePropertyAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = property.PropertyId }, property);
    }

    // Restricted: Modify existing property
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        await _propertyService.UpdatePropertyAsync(id, dto);
        return NoContent();
    }

    // Restricted: Remove a listing
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _propertyService.DeletePropertyAsync(id);
        return NoContent();
    }
    
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPost("{id}/pictures")]
    public async Task<IActionResult> AddPicture(Guid id, [FromBody] PropertyPictureDto dto)
    { 
        var picture = _mapper.Map<PropertyPicture>(dto);
        await _propertyService.AddPictureToPropertyAsync(id, picture);
    
        // Return the generated ID for immediate UI update on the frontend
        return Ok(new { id = picture.PropertyPictureId });
    }

    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPut("{id}/pictures/{pictureId}")]
    public async Task<IActionResult> UpdatePicture(Guid id, Guid pictureId, [FromBody] PropertyPictureDto dto)
    {
        var picture = _mapper.Map<PropertyPicture>(dto);
        picture.PropertyPictureId = pictureId; 
    
        await _propertyService.UpdatePictureToPropertyAsync(id, picture);
        return NoContent();
    }

    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpDelete("{propertyId}/pictures/{pictureId}")]
    public async Task<IActionResult> DeletePicture(Guid propertyId, Guid pictureId)
    {
        await _propertyService.DeletePictureToPropertyAsync(propertyId, pictureId);
        return NoContent();
    }
}