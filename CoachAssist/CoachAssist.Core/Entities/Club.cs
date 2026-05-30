namespace CoachAssist.Core.Entities;

public class Club
{
    public int ClubId { get; set; }               // PK
    public string Naam { get; set; } = string.Empty;
    public string Stad { get; set; } = string.Empty;

    // Navigatie
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Terrein> Terreinen { get; set; } = new List<Terrein>();
    public ICollection<ClubAanvraag> Aanvragen { get; set; } = new List<ClubAanvraag>();
}
