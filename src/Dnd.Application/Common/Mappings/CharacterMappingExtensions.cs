using Dnd.Application.DTOs.Characters;
using Dnd.Domain.Entities;

namespace Dnd.Application.Common.Mappings;

public static class CharacterMappingExtensions
{
    public static CharacterDto ToDto(this Character character)
    {
        return new CharacterDto(
            character.Id,
            character.Name,
            character.Class,
            character.Level,
            character.CurrentHp,
            character.MaxHp,
            character.CreatedAt
        );
    }

    public static Character ToEntity(this CreateCharacterDto dto)
    {
        return new Character
        {
            Id = string.IsNullOrWhiteSpace(dto.Id) ? Guid.NewGuid().ToString() : dto.Id,
            Name = dto.Name,
            Class = dto.Class,
            Level = dto.Level,
            CurrentHp = dto.CurrentHp,
            MaxHp = dto.MaxHp,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateFromDto(this Character character, UpdateCharacterDto dto)
    {
        character.Name = dto.Name;
        character.Class = dto.Class;
        character.Level = dto.Level;
        character.CurrentHp = dto.CurrentHp;
        character.MaxHp = dto.MaxHp;
    }
}

