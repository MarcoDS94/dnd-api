namespace Dnd.Application.DTOs.CampaignMembers;

public record CampaignMemberDto(
    Guid Campaignid,
    Guid Userid,
    int Roleid,
    DateTime Joinedat,
    string? Username = null,
    string? RoleSlug = null
);

