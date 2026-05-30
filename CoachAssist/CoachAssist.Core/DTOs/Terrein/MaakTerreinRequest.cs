using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Terrein;

public class MaakTerreinRequest
{
    [Required]
    public string Naam { get; set; } = string.Empty;

    [Required]
    public string Adres { get; set; } = string.Empty;

    [Required]
    public int ClubId { get; set; }
}
