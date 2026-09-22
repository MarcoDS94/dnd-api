namespace Dnd.Application.DTOs.Campaigns;

public record CampaignDto(
    Guid Id,
    string Title,
    string? Description,
    string? Coverimageurl,
    string CodInd,
    DateTime Createdat
);

