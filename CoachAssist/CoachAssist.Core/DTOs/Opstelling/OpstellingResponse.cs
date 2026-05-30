namespace CoachAssist.Core.DTOs.Opstelling;

public class OpstellingResponse
{
    public int OpstellingId { get; set; }
    public int? WedstrijdId { get; set; }
    public string Formatie { get; set; } = string.Empty;
    public List<OpstellingSpelerDetailDto> Spelers { get; set; } = new();
}
