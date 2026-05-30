namespace CoachAssist.Core.DTOs.Club;

public class ClubDetailResponse
{
    public int ClubId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Stad { get; set; } = string.Empty;
    public int AantalTeams { get; set; }
    public int AantalTerreinen { get; set; }
}
