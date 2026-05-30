using CoachAssist.Core.Enums;

namespace CoachAssist.Core.Entities;

public class Gebruiker
{
    public int GebruikerId { get; set; }          // PK
    public string Email { get; set; } = string.Empty;     // uniek, verplicht
    public string PasswordHash { get; set; } = string.Empty;
    public GebruikerRol Rol { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;

    // Navigatie
    public Speler? Speler { get; set; }                    // alleen als Rol == Speler
    public ICollection<Team> GecoachteTeams { get; set; } = new List<Team>();  // als Rol == Coach
}
