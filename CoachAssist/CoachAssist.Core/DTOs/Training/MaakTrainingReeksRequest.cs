using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Training;

public class MaakTrainingReeksRequest
{
    [Required] public DayOfWeek DagVanDeWeek { get; set; }
    [Required] public TimeSpan StartTijd { get; set; }
    [Required] public TimeSpan EindTijd { get; set; }
    [Required] public int TeamId { get; set; }
    [Required] public int TerreinId { get; set; }
    [Required] public DateTime EersteDatum { get; set; }
    [Required] public DateTime LaatsteDatum { get; set; }
}
