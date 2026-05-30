using System.ComponentModel.DataAnnotations;
using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Wachtwoord { get; set; } = string.Empty;

    [Required]
    public string Voornaam { get; set; } = string.Empty;

    [Required]
    public string Achternaam { get; set; } = string.Empty;

    [Required]
    public GebruikerRol Rol { get; set; }
}
