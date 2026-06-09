using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YMMO.Backend.Application.DTOs.Agent;
using YMMO.Backend.Application.Interfaces;
using YMMO.BackEnd.Application.Interfaces;

namespace YMMO.BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    // Accessible by the Agent, their Manager, or an Admin
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var agent = await _agentService.GetAgentDetailsAsync(id);
        return Ok(agent);
    }

    // Agent can update their own profile, Admin can also perform updates
    [Authorize(Roles = "Agent,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAgentDto dto)
    {
        await _agentService.UpdateAgentProfileAsync(id, dto);
        // 204 - Successful update without returning content
        return NoContent(); 
    }

    // Allows Agents, Managers, and Admins to view properties associated with an agent
    [Authorize(Roles = "Agent,Manager,Admin")]
    [HttpGet("{id}/properties")]
    public async Task<IActionResult> GetProperties(Guid id)
    {
        var properties = await _agentService.GetAgentPropertiesAsync(id);
        return Ok(properties);
    }
}