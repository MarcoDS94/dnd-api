using Dnd.Application.DTOs.Users;

namespace Dnd.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    UserDto User
);

