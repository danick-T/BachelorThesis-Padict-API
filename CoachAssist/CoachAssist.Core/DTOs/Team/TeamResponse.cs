namespace CoachAssist.Core.DTOs.Team;

public class TeamResponse
{
    public int TeamId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Seizoen { get; set; } = string.Empty;
    public int ClubId { get; set; }
    public int? CoachId { get; set; }
    public string? CoachNaam { get; set; }
}
