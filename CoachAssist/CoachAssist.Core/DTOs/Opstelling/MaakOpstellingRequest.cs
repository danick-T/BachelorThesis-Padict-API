using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Opstelling;

public class MaakOpstellingRequest
{
    [Required]
    public int WedstrijdId { get; set; }

    [Required]
    public string Formatie { get; set; } = string.Empty;

    [Required]
    public List<OpstellingSpelerDto> Spelers { get; set; } = new();
}
