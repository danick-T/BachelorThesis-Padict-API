namespace CoachAssist.Core.DTOs.Statistiek;

public class SeizoenTeamOverzicht
{
    public int TeamId { get; set; }
    public string TeamNaam { get; set; } = string.Empty;
    public string Seizoen { get; set; } = string.Empty;
    public List<SeizoenSpelerStats> Spelers { get; set; } = new();
}
