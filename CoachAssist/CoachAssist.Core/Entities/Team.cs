namespace CoachAssist.Core.Entities;

public class Team
{
    public int TeamId { get; set; }               // PK
    public string Naam { get; set; } = string.Empty;     // bv. "U17b BEERSEL"
    public string Seizoen { get; set; } = string.Empty;  // bv. "2025-2026"
    public int ClubId { get; set; }               // FK → Club
    public int? CoachId { get; set; }             // FK naar Gebruiker (Rol == Coach)

    // Navigatie
    public Club Club { get; set; } = null!;
    public Gebruiker? Coach { get; set; }
    public ICollection<Speler> Spelers { get; set; } = new List<Speler>();
    public ICollection<Wedstrijd> Wedstrijden { get; set; } = new List<Wedstrijd>();
    public ICollection<Training> Trainingen { get; set; } = new List<Training>();
    public ICollection<TeamAanvraag> Aanvragen { get; set; } = new List<TeamAanvraag>();
}
