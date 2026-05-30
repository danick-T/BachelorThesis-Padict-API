using CoachAssist.Core.Enums;

namespace CoachAssist.Core.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime VerlooptOp { get; set; }
    public CurrentUserResponse Gebruiker { get; set; } = new();
}

public class CurrentUserResponse
{
    public int GebruikerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public GebruikerRol Rol { get; set; }
}
