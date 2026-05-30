using CoachAssist.Core.DTOs.Auth;

namespace CoachAssist.Shared.Services;

public interface IAuthSession
{
    string? Token { get; }
    DateTime? VerlooptOp { get; }
    CurrentUserResponse? Gebruiker { get; }
    bool IsIngelogd { get; }

    void SetAuth(AuthResponse auth);
    void SetGebruiker(CurrentUserResponse gebruiker);
    void Clear();
}
