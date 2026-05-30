using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Aanvraag;

public class MaakTeamAanvraagRequest
{
    [Required]
    public int TeamId { get; set; }
}
