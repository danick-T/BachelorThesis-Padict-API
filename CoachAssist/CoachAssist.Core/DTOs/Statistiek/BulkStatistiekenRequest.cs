namespace CoachAssist.Core.DTOs.Statistiek;

public class BulkStatistiekenRequest
{
    public List<SpelerStatistiekDto> Statistieken { get; set; } = new();
    public int? ThuisScore { get; set; }
    public int? UitScore { get; set; }
}
