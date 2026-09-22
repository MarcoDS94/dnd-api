using Dnd.Application.DTOs.Users;
using Dnd.Domain.Entities;

namespace Dnd.Application.Common.Mappings;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Createdat
        );
    }
}

