using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Terrein;

public class WerkTerreinBijRequest
{
    [Required]
    public string Naam { get; set; } = string.Empty;

    [Required]
    public string Adres { get; set; } = string.Empty;
}
