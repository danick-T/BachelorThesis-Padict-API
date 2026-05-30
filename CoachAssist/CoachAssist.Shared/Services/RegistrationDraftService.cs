namespace CoachAssist.Shared.Services;

public sealed class RegistrationDraftService
{
    public string Voornaam { get; private set; } = string.Empty;
    public string Achternaam { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Wachtwoord { get; private set; } = string.Empty;
    public bool HasDraft => !string.IsNullOrWhiteSpace(Email);

    public void Set(string voornaam, string achternaam, string email, string wachtwoord)
    {
        Voornaam = voornaam.Trim();
        Achternaam = achternaam.Trim();
        Email = email.Trim();
        Wachtwoord = wachtwoord;
    }

    public void Clear()
    {
        Voornaam = string.Empty;
        Achternaam = string.Empty;
        Email = string.Empty;
        Wachtwoord = string.Empty;
    }
}
