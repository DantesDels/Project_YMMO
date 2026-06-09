using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Property;
using YMMO.Backend.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropertySummaryDto>), 200)]
    public async Task<IActionResult> Search([FromQuery] PropertySearchCriteriaDto criteria)
    {
        // The service handles the complex filtering logic
        var results = await _searchService.SearchAsync(criteria);
        
        return Ok(results);
    }
    
    [HttpGet("suggestions/{propertyId}")]
    public async Task<IActionResult> GetSuggestions(Guid propertyId)
    {
        var suggestions = await _searchService.GetSimilarPropertiesAsync(propertyId, 5);
        return Ok(suggestions);
    }
}