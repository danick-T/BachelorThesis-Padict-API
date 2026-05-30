namespace CoachAssist.Core.DTOs.Terrein;

public class TerreinResponse
{
    public int TerreinId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public int ClubId { get; set; }
}
