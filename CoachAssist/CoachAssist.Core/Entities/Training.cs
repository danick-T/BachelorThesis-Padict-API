using CoachAssist.Core.Enums;

namespace CoachAssist.Core.Entities;

public class Training
{
    public int TrainingId { get; set; }           // PK
    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }
    public TimeSpan EindTijd { get; set; }
    public int TeamId { get; set; }               // FK → Team
    public int TerreinId { get; set; }            // FK naar Terrein
    public TrainingStatus Status { get; set; }
    public string? Opmerking { get; set; }

    // Navigatie
    public Team Team { get; set; } = null!;
    public Terrein Terrein { get; set; } = null!;
    public ICollection<TrainingAanwezigheid> Aanwezigheden { get; set; } = new List<TrainingAanwezigheid>();
}
