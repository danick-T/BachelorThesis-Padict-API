using CoachAssist.Core.Enums;

namespace CoachAssist.Core.Entities;

public class TeamAanvraag
{
    public int AanvraagId { get; set; }           // PK
    public int SpelerId { get; set; }             // FK → Speler
    public int TeamId { get; set; }               // FK → Team
    public AanvraagStatus Status { get; set; }
    public DateTime? DatumAanvraag { get; set; }

    // Navigatie
    public Speler Speler { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
