using Dnd.Domain.Entities;

namespace Dnd.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, out DateTime expiresAt);
}

