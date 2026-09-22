using Dnd.Application.DTOs.Campaigns;
using Dnd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dnd.Api.Controllers;

[Authorize]
[EnableRateLimiting("general-policy")]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    /// <summary>
    /// Retrieves all campaigns.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CampaignDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CampaignDto>>> GetAll(CancellationToken cancellationToken)
    {
        var campaigns = await _campaignService.GetAllAsync(cancellationToken);
        return Ok(campaigns);
    }

    /// <summary>
    /// Retrieves a specific campaign by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampaignDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var campaign = await _campaignService.GetByIdAsync(id, cancellationToken);
        if (campaign == null)
        {
            return NotFound(new { message = $"Campaign with ID '{id}' was not found." });
        }

        return Ok(campaign);
    }

    /// <summary>
    /// Creates a new campaign.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CampaignDto>> Create([FromBody] CreateCampaignDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest(new { message = "Campaign title is required." });
        }

        var created = await _campaignService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing campaign.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CampaignDto>> Update(Guid id, [FromBody] UpdateCampaignDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest(new { message = "Campaign title is required." });
        }

        var updated = await _campaignService.UpdateAsync(id, dto, cancellationToken);
        if (updated == null)
        {
            return NotFound(new { message = $"Campaign with ID '{id}' was not found." });
        }

        return Ok(updated);
    }

    /// <summary>
    /// Deletes a campaign by ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _campaignService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = $"Campaign with ID '{id}' was not found." });
        }

        return NoContent();
    }
}

