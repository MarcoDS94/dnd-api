namespace Dnd.Application.DTOs.Characters;

public record CreateCharacterDto(
    string? Id,
    string Name,
    string Class,
    int Level,
    int CurrentHp,
    int MaxHp
);

