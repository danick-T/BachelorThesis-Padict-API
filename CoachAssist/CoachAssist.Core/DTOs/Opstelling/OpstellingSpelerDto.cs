using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Opstelling;

public class OpstellingSpelerDto
{
    public int SpelerId { get; set; }
    public OpstellingPositie PositieNaam { get; set; }
    public int? VeldPositieX { get; set; }
    public int? VeldPositieY { get; set; }
}
