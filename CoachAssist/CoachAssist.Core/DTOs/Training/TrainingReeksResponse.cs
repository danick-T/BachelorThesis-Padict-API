namespace CoachAssist.Core.DTOs.Training;

public class TrainingReeksResponse
{
    public int Aangemaakt { get; set; }
    public List<DateTime> Overgeslagen { get; set; } = new();
    public List<TrainingResponse> Nieuwe { get; set; } = new();
}
