namespace CoachAssist.Core.Entities;

using CoachAssist.Core.Enums;

public class OpstellingSpeler
{
    public int OpstellingSpelerId { get; set; }   // PK
    public int OpstellingId { get; set; }         // FK → Opstelling
    public int SpelerId { get; set; }             // FK → Speler
    public OpstellingPositie PositieNaam { get; set; }
    public int? VeldPositieX { get; set; }
    public int? VeldPositieY { get; set; }

    // Navigatie
    public Opstelling Opstelling { get; set; } = null!;
    public Speler Speler { get; set; } = null!;
}
