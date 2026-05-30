using CoachAssist.Core.DTOs.Auth;

namespace CoachAssist.Shared.Services;

public sealed class AuthSession : IAuthSession
{
    public string? Token { get; private set; }
    public DateTime? VerlooptOp { get; private set; }
    public CurrentUserResponse? Gebruiker { get; private set; }
    public bool IsIngelogd => !string.IsNullOrWhiteSpace(Token);

    public void SetAuth(AuthResponse auth)
    {
        Token = auth.Token;
        VerlooptOp = auth.VerlooptOp;
        Gebruiker = auth.Gebruiker;
    }

    public void SetGebruiker(CurrentUserResponse gebruiker)
    {
        Gebruiker = gebruiker;
    }

    public void Clear()
    {
        Token = null;
        VerlooptOp = null;
        Gebruiker = null;
    }
}
