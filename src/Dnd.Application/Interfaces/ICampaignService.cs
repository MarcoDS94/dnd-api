using Dnd.Application.DTOs.Campaigns;

namespace Dnd.Application.Interfaces;

public interface ICampaignService
{
    Task<IEnumerable<CampaignDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CampaignDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CampaignDto> CreateAsync(CreateCampaignDto dto, CancellationToken cancellationToken = default);
    Task<CampaignDto?> UpdateAsync(Guid id, UpdateCampaignDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

