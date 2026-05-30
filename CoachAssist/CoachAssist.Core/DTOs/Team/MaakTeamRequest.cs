using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Team;

public class MaakTeamRequest
{
    [Required]
    public string Naam { get; set; } = string.Empty;

    [Required]
    public string Seizoen { get; set; } = string.Empty;

    [Required]
    public int ClubId { get; set; }

    public int? CoachId { get; set; }
}
