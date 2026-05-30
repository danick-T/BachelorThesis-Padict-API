namespace CoachAssist.Core.DTOs.Aanwezigheid;

public class AanwezigheidResponse
{
    public int AanwezigheidId { get; set; }
    public int TrainingId { get; set; }
    public int SpelerId { get; set; }
    public string SpelerNaam { get; set; } = string.Empty;
    public bool Aanwezig { get; set; }
    public string? Reden { get; set; }
}
