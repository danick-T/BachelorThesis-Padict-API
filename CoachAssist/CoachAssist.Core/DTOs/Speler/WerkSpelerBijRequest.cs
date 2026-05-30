using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Speler;

public class WerkSpelerBijRequest
{
    public string? Voornaam { get; set; }
    public string? Achternaam { get; set; }
    public int? Rugnummer { get; set; }
    public VastePositie? VastePositie { get; set; }
}
