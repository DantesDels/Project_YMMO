using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
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
        return CreatedAtAction(nameof(GetById), new { id = property.PropertyID }, property);
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
}