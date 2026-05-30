using System.ComponentModel.DataAnnotations;

namespace CoachAssist.Core.DTOs.Aanvraag;

public class MaakClubAanvraagRequest
{
    [Required]
    public int ClubId { get; set; }
}
