using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Agency;
using YMMO.Backend.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgencyController : ControllerBase
{
    private readonly IAgencyService _agencyService;

    public AgencyController(IAgencyService agencyService)
    {
        _agencyService = agencyService;
    }

    // Agencies are public entities
    [HttpGet]
    public async Task<IActionResult> GetAllAgencies()
    {
        var agencies = await _agencyService.GetAllAgenciesAsync();
        return Ok(agencies);
    }

    // Only Managers and Admins can update agency details
    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAgency(Guid id, [FromBody] UpdateAgencyDto dto)
    {
        await _agencyService.UpdateAgencyAsync(id, dto);
        return NoContent();
    }
}