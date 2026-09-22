using Dnd.Application.DTOs.Roles;
using Dnd.Domain.Entities;

namespace Dnd.Application.Common.Mappings;

public static class RoleMappingExtensions
{
    public static RoleDto ToDto(this Role role)
    {
        return new RoleDto(
            role.Id,
            role.Slug,
            role.Descrizione
        );
    }

    public static Role ToEntity(this CreateRoleDto dto)
    {
        return new Role
        {
            Slug = dto.Slug,
            Descrizione = dto.Descrizione
        };
    }
}

