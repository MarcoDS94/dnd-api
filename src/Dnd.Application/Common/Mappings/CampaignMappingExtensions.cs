using Dnd.Application.DTOs.Campaigns;
using Dnd.Domain.Entities;

namespace Dnd.Application.Common.Mappings;

public static class CampaignMappingExtensions
{
    public static CampaignDto ToDto(this Campaign campaign)
    {
        return new CampaignDto(
            campaign.Id,
            campaign.Title,
            campaign.Description,
            campaign.Coverimageurl,
            campaign.CodInd,
            campaign.Createdat
        );
    }

    public static Campaign ToEntity(this CreateCampaignDto dto)
    {
        return new Campaign
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Coverimageurl = dto.Coverimageurl,
            CodInd = string.IsNullOrWhiteSpace(dto.CodInd) ? GenerateCode() : dto.CodInd,
            Createdat = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this Campaign campaign, UpdateCampaignDto dto)
    {
        campaign.Title = dto.Title;
        campaign.Description = dto.Description;
        campaign.Coverimageurl = dto.Coverimageurl;
    }

    private static string GenerateCode() => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
}

