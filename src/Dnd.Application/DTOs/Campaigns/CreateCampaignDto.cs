using System.ComponentModel.DataAnnotations;

namespace Dnd.Application.DTOs.Campaigns;

public record CreateCampaignDto(
    [Required(ErrorMessage = "Campaign title is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Campaign title must be between 1 and 100 characters.")]
    string Title,

    [StringLength(2000, ErrorMessage = "Campaign description cannot exceed 2000 characters.")]
    string? Description,

    [StringLength(255, ErrorMessage = "Cover image URL cannot exceed 255 characters.")]
    [Url(ErrorMessage = "Cover image URL must be a valid URL format.")]
    string? Coverimageurl,

    [StringLength(20, ErrorMessage = "Campaign code cannot exceed 20 characters.")]
    string? CodInd
);
