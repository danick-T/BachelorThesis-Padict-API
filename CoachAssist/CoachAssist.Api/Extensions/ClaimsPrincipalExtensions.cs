using System.Security.Claims;
using CoachAssist.Core.Enums;

namespace CoachAssist.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetGebruikerId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var id))
            throw new InvalidOperationException("Ingelogde gebruiker heeft geen geldig ID-claim.");
        return id;
    }

    public static int? TryGetGebruikerId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    public static GebruikerRol GetRol(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.Role);
        if (!Enum.TryParse<GebruikerRol>(claim, out var rol))
            throw new InvalidOperationException("Ingelogde gebruiker heeft geen geldige rol-claim.");
        return rol;
    }

    public static bool IsInRol(this ClaimsPrincipal user, GebruikerRol rol)
        => user.FindFirstValue(ClaimTypes.Role) == rol.ToString();
}
