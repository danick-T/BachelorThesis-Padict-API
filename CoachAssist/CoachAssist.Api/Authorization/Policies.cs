using CoachAssist.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CoachAssist.Api.Authorization;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string CoachOnly = "CoachOnly";
    public const string SpelerOnly = "SpelerOnly";
    public const string AdminOfCoach = "AdminOfCoach";

    public static AuthorizationOptions VoegCoachAssistPoliciesToe(this AuthorizationOptions opt)
    {
        opt.AddPolicy(AdminOnly, p => p.RequireRole(GebruikerRol.Admin.ToString()));
        opt.AddPolicy(CoachOnly, p => p.RequireRole(GebruikerRol.Coach.ToString()));
        opt.AddPolicy(SpelerOnly, p => p.RequireRole(GebruikerRol.Speler.ToString()));
        opt.AddPolicy(AdminOfCoach, p => p.RequireRole(
            GebruikerRol.Admin.ToString(),
            GebruikerRol.Coach.ToString()));
        return opt;
    }
}
