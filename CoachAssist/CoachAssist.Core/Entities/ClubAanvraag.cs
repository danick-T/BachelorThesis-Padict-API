using CoachAssist.Core.Enums;

namespace CoachAssist.Core.Entities;

public class ClubAanvraag
{
    public int AanvraagId { get; set; }           // PK
    public int GebruikerId { get; set; }          // FK → Gebruiker
    public int ClubId { get; set; }               // FK → Club
    public AanvraagStatus Status { get; set; }
    public DateTime? DatumAanvraag { get; set; }

    // Navigatie
    public Gebruiker Gebruiker { get; set; } = null!;
    public Club Club { get; set; } = null!;
}
