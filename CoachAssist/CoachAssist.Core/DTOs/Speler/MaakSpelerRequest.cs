using System.ComponentModel.DataAnnotations;
using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Speler;

public class MaakSpelerRequest
{
    [Required]
    public string Voornaam { get; set; } = string.Empty;

    [Required]
    public string Achternaam { get; set; } = string.Empty;

    public int? Rugnummer { get; set; }

    public VastePositie? VastePositie { get; set; }

    [Required]
    public int TeamId { get; set; }
}
