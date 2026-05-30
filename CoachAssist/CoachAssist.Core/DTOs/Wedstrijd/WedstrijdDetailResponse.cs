using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Wedstrijd;

public class WedstrijdDetailResponse
{
    public int WedstrijdId { get; set; }
    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }
    public string Tegenstander { get; set; } = string.Empty;
    public int? ThuisScore { get; set; }
    public int? UitScore { get; set; }
    public int TeamId { get; set; }
    public int TerreinId { get; set; }
    public WedstrijdStatus Status { get; set; }
    public string? Opmerking { get; set; }
    public string TeamNaam { get; set; } = string.Empty;
    public string TerreinNaam { get; set; } = string.Empty;
    public bool HeeftOpstelling { get; set; }
}
