using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Offer;
using YMMO.Backend.Application.DTOs.Offers;
using YMMO.Backend.Application.Interfaces;
using YMMO.BackEnd.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    // Clients create offers, Agents/Managers review them
    [Authorize(Roles = "Client,Agent,Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateOffer([FromBody] CreateOfferDto dto)
    {
        var offer = await _offerService.CreateOfferAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = offer.OfferID, propertyId = offer.PropertyID }, offer);
    }

    // Agents and Managers view offer details
    // Note: Added propertyId as a query parameter as required by your interface signature
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] Guid propertyId)
    {
        var offer = await _offerService.GetOfferDetailsAsync(id, propertyId);
        return Ok(offer);
    }

    // Agents/Managers update the status
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusOfferDto dto)
    {
        await _offerService.UpdateStatusOfferAsync(id, dto);
        return NoContent();
    }
}