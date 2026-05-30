using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Aanvraag;

public class ClubAanvraagResponse
{
    public int AanvraagId { get; set; }
    public int GebruikerId { get; set; }
    public string GebruikerNaam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ClubId { get; set; }
    public string ClubNaam { get; set; } = string.Empty;
    public AanvraagStatus Status { get; set; }
    public DateTime? DatumAanvraag { get; set; }
}
