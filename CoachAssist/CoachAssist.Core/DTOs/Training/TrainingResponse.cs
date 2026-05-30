using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Training;

public class TrainingResponse
{
    public int TrainingId { get; set; }
    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }
    public TimeSpan EindTijd { get; set; }
    public int TeamId { get; set; }
    public int TerreinId { get; set; }
    public TrainingStatus Status { get; set; }
    public string? Opmerking { get; set; }
    public string TerreinNaam { get; set; } = string.Empty;
}
