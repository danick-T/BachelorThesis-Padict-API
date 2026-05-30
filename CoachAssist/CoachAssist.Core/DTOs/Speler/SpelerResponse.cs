using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Speler;

public class SpelerResponse
{
    public int SpelerId { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public int? Rugnummer { get; set; }
    public VastePositie? VastePositie { get; set; }
    public int TeamId { get; set; }
}
