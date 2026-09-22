namespace Dnd.Application.DTOs.Users;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    DateTime Createdat
);

