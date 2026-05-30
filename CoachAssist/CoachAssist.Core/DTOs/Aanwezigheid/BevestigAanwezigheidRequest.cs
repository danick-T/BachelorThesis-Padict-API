using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Aanwezigheid;

public class BevestigAanwezigheidRequest
{
    [Required] public int TrainingId { get; set; }
    [Required] public bool Aanwezig { get; set; }
    public string? Reden { get; set; }
}
