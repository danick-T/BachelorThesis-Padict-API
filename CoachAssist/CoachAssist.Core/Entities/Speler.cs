namespace CoachAssist.Core.Entities;

using CoachAssist.Core.Enums;

public class Speler
{
    public int SpelerId { get; set; }             // PK
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public int? Rugnummer { get; set; }
    public VastePositie? VastePositie { get; set; }
    public int TeamId { get; set; }               // FK naar Team
    public int? GebruikerId { get; set; }         // FK → Gebruiker (nullable: handmatig toegevoegd)

    // Navigatie
    public Team Team { get; set; } = null!;
    public Gebruiker? Gebruiker { get; set; }
    public ICollection<SpelerStatistiek> Statistieken { get; set; } = new List<SpelerStatistiek>();
    public ICollection<TrainingAanwezigheid> Aanwezigheden { get; set; } = new List<TrainingAanwezigheid>();
    public ICollection<OpstellingSpeler> Opstellingen { get; set; } = new List<OpstellingSpeler>();
}
