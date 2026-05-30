namespace CoachAssist.Core.DTOs.Statistiek;

public class StatistiekResponse
{
    public int StatistiekId { get; set; }
    public int SpelerId { get; set; }
    public string SpelerNaam { get; set; } = string.Empty;
    public int WedstrijdId { get; set; }
    public int? Goals { get; set; }
    public int? Assists { get; set; }
    public int? GeleKaarten { get; set; }
    public int? RodeKaarten { get; set; }
    public int? GespeeldeMinuten { get; set; }
}
