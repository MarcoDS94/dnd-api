using Dnd.Application.Common.Mappings;
using Dnd.Application.DTOs.Campaigns;
using Dnd.Application.Interfaces;
using Dnd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dnd.Infrastructure.Services;

public class CampaignService : ICampaignService
{
    private readonly AppDbContext _context;

    public CampaignService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CampaignDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var campaigns = await _context.Campaigns
            .AsNoTracking()
            .OrderByDescending(c => c.Createdat)
            .ToListAsync(cancellationToken);

        return campaigns.Select(c => c.ToDto());
    }

    public async Task<CampaignDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await _context.Campaigns
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return campaign?.ToDto();
    }

    public async Task<CampaignDto> CreateAsync(CreateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.ToEntity();
        _context.Campaigns.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }

    public async Task<CampaignDto?> UpdateAsync(Guid id, UpdateCampaignDto dto, CancellationToken cancellationToken = default)
    {
        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (campaign == null)
        {
            return null;
        }

        campaign.UpdateFromDto(dto);
        await _context.SaveChangesAsync(cancellationToken);

        return campaign.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (campaign == null)
        {
            return false;
        }

        _context.Campaigns.Remove(campaign);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

