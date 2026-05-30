namespace CoachAssist.Core.Entities;

// Unique constraint: WedstrijdId — zie AppDbContext
public class Opstelling
{
    public int OpstellingId { get; set; }         // PK
    public int? WedstrijdId { get; set; }         // FK naar Wedstrijd (uniek)
    public string Formatie { get; set; } = string.Empty;  // bv. "4-3-3"

    // Navigatie
    public Wedstrijd? Wedstrijd { get; set; }
    public ICollection<OpstellingSpeler> Spelers { get; set; } = new List<OpstellingSpeler>();
}
