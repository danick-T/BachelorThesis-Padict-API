using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Aanvraag;

public class TeamAanvraagResponse
{
    public int AanvraagId { get; set; }
    public int SpelerId { get; set; }
    public string SpelerNaam { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamNaam { get; set; } = string.Empty;
    public AanvraagStatus Status { get; set; }
    public DateTime? DatumAanvraag { get; set; }
}
