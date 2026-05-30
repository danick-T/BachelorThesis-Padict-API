namespace CoachAssist.Core.DTOs.Statistiek;

public class SeizoenSpelerStats
{
    public int SpelerId { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public int? Rugnummer { get; set; }
    public int TotaalGoals { get; set; }
    public int TotaalAssists { get; set; }
    public int TotaalGeleKaarten { get; set; }
    public int TotaalRodeKaarten { get; set; }
    public int TotaalMinuten { get; set; }
    public int AantalWedstrijden { get; set; }
}
