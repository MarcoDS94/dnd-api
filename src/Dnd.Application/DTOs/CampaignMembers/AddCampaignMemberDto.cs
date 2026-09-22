namespace Dnd.Application.DTOs.CampaignMembers;

public record AddCampaignMemberDto(
    Guid Campaignid,
    Guid Userid,
    int Roleid
);

