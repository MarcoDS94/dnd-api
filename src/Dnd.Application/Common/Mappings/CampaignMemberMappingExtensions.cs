using Dnd.Application.DTOs.CampaignMembers;
using Dnd.Domain.Entities;

namespace Dnd.Application.Common.Mappings;

public static class CampaignMemberMappingExtensions
{
    public static CampaignMemberDto ToDto(this Campaignmember member)
    {
        return new CampaignMemberDto(
            member.Campaignid,
            member.Userid,
            member.Roleid,
            member.Joinedat,
            member.User?.Username,
            member.Role?.Slug
        );
    }

    public static Campaignmember ToEntity(this AddCampaignMemberDto dto)
    {
        return new Campaignmember
        {
            Campaignid = dto.Campaignid,
            Userid = dto.Userid,
            Roleid = dto.Roleid,
            Joinedat = DateTime.UtcNow
        };
    }
}

