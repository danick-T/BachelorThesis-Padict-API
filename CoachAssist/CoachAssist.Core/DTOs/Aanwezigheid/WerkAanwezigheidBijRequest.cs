using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Aanwezigheid;

public class WerkAanwezigheidBijRequest
{
    [Required] public bool Aanwezig { get; set; }
    public string? Reden { get; set; }
}
