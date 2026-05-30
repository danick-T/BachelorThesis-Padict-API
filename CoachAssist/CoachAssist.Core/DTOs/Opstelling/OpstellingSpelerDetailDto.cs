using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Opstelling;

public class OpstellingSpelerDetailDto
{
    public int SpelerId { get; set; }
    public OpstellingPositie PositieNaam { get; set; }
    public int? VeldPositieX { get; set; }
    public int? VeldPositieY { get; set; }
    public string SpelerNaam { get; set; } = string.Empty;
    public int? Rugnummer { get; set; }
}
