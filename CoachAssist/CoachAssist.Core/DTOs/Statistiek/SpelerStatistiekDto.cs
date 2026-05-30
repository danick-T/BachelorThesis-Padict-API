namespace CoachAssist.Core.DTOs.Statistiek;

public class SpelerStatistiekDto
{
    public int SpelerId { get; set; }
    public int? Goals { get; set; }
    public int? Assists { get; set; }
    public int? GeleKaarten { get; set; }
    public int? RodeKaarten { get; set; }
    public int? GespeeldeMinuten { get; set; }
}
