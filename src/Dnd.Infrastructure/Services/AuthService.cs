using Dnd.Application.Common.Exceptions;
using Dnd.Application.Common.Mappings;
using Dnd.Application.DTOs.Auth;
using Dnd.Application.DTOs.Users;
using Dnd.Application.Interfaces;
using Dnd.Domain.Entities;
using Dnd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dnd.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // Pre-computed constant hash to ensure failed login attempts run in constant time
    // and cannot be used for username enumeration timing attacks.
    private const string DummyBcryptHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";

    public AuthService(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        ValidatePasswordComplexity(dto.Password);

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var normalizedUsername = dto.Username.Trim();

        var existingUser = await _context.Users
            .Where(u => u.Email.ToLower() == normalizedEmail || u.Username.ToLower() == normalizedUsername.ToLower())
            .Select(u => new { u.Email, u.Username })
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser != null)
        {
            if (string.Equals(existingUser.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserAlreadyExistsException($"A user with email '{dto.Email}' already exists.");
            }
            throw new UserAlreadyExistsException($"A user with username '{dto.Username}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = normalizedUsername,
            Email = normalizedEmail,
            Passwordhash = _passwordHasher.HashPassword(dto.Password),
            Createdat = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);
        return new AuthResponseDto(token, expiresAt, user.ToDto());
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var identifier = dto.UsernameOrEmail.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == identifier || u.Email.ToLower() == identifier.ToLower(), cancellationToken);

        if (user == null)
        {
            // Execute dummy verify to equalize execution time and prevent user enumeration
            _passwordHasher.VerifyPassword(dto.Password, DummyBcryptHash);
            return null;
        }

        var isValidPassword = _passwordHasher.VerifyPassword(dto.Password, user.Passwordhash);
        if (!isValidPassword)
        {
            return null;
        }

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAt);
        return new AuthResponseDto(token, expiresAt, user.ToDto());
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.ToDto();
    }

    private static void ValidatePasswordComplexity(string password)
    {
        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
        {
            throw new PasswordComplexityException(
                "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }
    }
}
