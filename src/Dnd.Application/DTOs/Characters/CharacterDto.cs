namespace Dnd.Application.DTOs.Characters;

public record CharacterDto(
    string Id,
    string Name,
    string Class,
    int Level,
    int CurrentHp,
    int MaxHp,
    DateTime? CreatedAt
);

