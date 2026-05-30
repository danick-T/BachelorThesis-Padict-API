namespace CoachAssist.Core.Entities;

// Unique constraint: (TrainingId, SpelerId) — zie AppDbContext
public class TrainingAanwezigheid
{
    public int AanwezigheidId { get; set; }       // PK
    public int TrainingId { get; set; }           // FK → Training
    public int SpelerId { get; set; }             // FK → Speler
    public bool Aanwezig { get; set; }
    public string? Reden { get; set; }            // alleen ingevuld bij afwezigheid

    // Navigatie
    public Training Training { get; set; } = null!;
    public Speler Speler { get; set; } = null!;
}
