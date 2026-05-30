namespace CoachAssist.Core.Entities;

// Unique constraint: (SpelerId, WedstrijdId) — zie AppDbContext
public class SpelerStatistiek
{
    public int StatistiekId { get; set; }         // PK
    public int SpelerId { get; set; }             // FK → Speler
    public int WedstrijdId { get; set; }          // FK → Wedstrijd
    public int? Goals { get; set; }
    public int? Assists { get; set; }
    public int? GeleKaarten { get; set; }
    public int? RodeKaarten { get; set; }
    public int? GespeeldeMinuten { get; set; }

    // Navigatie
    public Speler Speler { get; set; } = null!;
    public Wedstrijd Wedstrijd { get; set; } = null!;
}
