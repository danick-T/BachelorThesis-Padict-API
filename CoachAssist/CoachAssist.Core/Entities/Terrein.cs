namespace CoachAssist.Core.Entities;

public class Terrein
{
    public int TerreinId { get; set; }            // PK
    public string Naam { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public int ClubId { get; set; }               // FK → Club

    // Navigatie
    public Club Club { get; set; } = null!;
    public ICollection<Training> Trainingen { get; set; } = new List<Training>();
    public ICollection<Wedstrijd> Wedstrijden { get; set; } = new List<Wedstrijd>();
}
