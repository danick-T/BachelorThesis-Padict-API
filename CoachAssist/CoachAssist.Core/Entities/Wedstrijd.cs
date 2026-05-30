using CoachAssist.Core.Enums;

namespace CoachAssist.Core.Entities;

public class Wedstrijd
{
    public int WedstrijdId { get; set; }          // PK
    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }
    public string Tegenstander { get; set; } = string.Empty;
    public int? ThuisScore { get; set; }          // null = nog niet gespeeld
    public int? UitScore { get; set; }            // null = nog niet gespeeld
    public int TeamId { get; set; }               // FK → Team
    public int TerreinId { get; set; }            // FK naar Terrein
    public WedstrijdStatus Status { get; set; }
    public string? Opmerking { get; set; }        // reden wijziging

    // Navigatie
    public Team Team { get; set; } = null!;
    public Terrein Terrein { get; set; } = null!;
    public ICollection<SpelerStatistiek> Statistieken { get; set; } = new List<SpelerStatistiek>();
    public Opstelling? Opstelling { get; set; }
}
