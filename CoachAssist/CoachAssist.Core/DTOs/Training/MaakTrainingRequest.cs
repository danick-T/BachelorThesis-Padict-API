using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Training;

public class MaakTrainingRequest
{
    [Required] public DateTime Datum { get; set; }
    [Required] public TimeSpan StartTijd { get; set; }
    [Required] public TimeSpan EindTijd { get; set; }
    [Required] public int TeamId { get; set; }
    [Required] public int TerreinId { get; set; }
    public string? Opmerking { get; set; }
}
