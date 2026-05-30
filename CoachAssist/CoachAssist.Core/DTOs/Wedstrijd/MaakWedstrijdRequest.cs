using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Wedstrijd;

public class MaakWedstrijdRequest
{
    [Required]
    public DateTime Datum { get; set; }

    [Required]
    public TimeSpan StartTijd { get; set; }

    [Required]
    public string Tegenstander { get; set; } = string.Empty;

    [Required]
    public int TeamId { get; set; }

    [Required]
    public int TerreinId { get; set; }
}
