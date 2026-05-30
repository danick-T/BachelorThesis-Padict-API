using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Club;

public class MaakClubRequest
{
    [Required]
    public string Naam { get; set; } = string.Empty;

    [Required]
    public string Stad { get; set; } = string.Empty;
}
