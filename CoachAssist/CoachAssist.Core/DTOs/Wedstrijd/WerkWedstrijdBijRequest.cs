using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Wedstrijd;

public class WerkWedstrijdBijRequest
{
    public DateTime? Datum { get; set; }
    public TimeSpan? StartTijd { get; set; }
    public string? Tegenstander { get; set; }
    public int? TeamId { get; set; }
    public int? TerreinId { get; set; }
    public int? ThuisScore { get; set; }
    public int? UitScore { get; set; }
    public WedstrijdStatus? Status { get; set; }
    public string? Opmerking { get; set; }
}
