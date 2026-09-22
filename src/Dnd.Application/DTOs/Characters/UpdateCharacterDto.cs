namespace Dnd.Application.DTOs.Characters;

public record UpdateCharacterDto(
    string Name,
    string Class,
    int Level,
    int CurrentHp,
    int MaxHp
);

