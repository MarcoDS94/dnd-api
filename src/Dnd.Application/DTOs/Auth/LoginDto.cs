using System.ComponentModel.DataAnnotations;

namespace Dnd.Application.DTOs.Auth;

public record LoginDto(
    [Required(ErrorMessage = "Username or email is required.")]
    [StringLength(255, ErrorMessage = "Username or email cannot exceed 255 characters.")]
    string UsernameOrEmail,

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    string Password
);
