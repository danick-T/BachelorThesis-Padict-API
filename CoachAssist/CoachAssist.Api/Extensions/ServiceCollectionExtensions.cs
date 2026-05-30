using CoachAssist.Api.Services;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data.Repositories;

namespace CoachAssist.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Slice A: Club & Terrein
        services.AddScoped<IClubRepository, ClubRepository>();
        services.AddScoped<ITerreinRepository, TerreinRepository>();

        // Slice B: Team & Speler
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ISpelerRepository, SpelerRepository>();

        // Slice C: Aanvragen
        services.AddScoped<IAanvraagRepository, AanvraagRepository>();

        // Slice D: Wedstrijden, Opstellingen, Statistieken
        services.AddScoped<IWedstrijdRepository, WedstrijdRepository>();
        services.AddScoped<IOpstellingRepository, OpstellingRepository>();
        services.AddScoped<IStatistiekRepository, StatistiekRepository>();
        services.AddScoped<StatistiekService>();

        // Slice E: Trainingen, Aanwezigheid, PlanningService
        services.AddScoped<ITrainingRepository, TrainingRepository>();
        services.AddScoped<IAanwezigheidRepository, AanwezigheidRepository>();
        services.AddScoped<IPlanningService, PlanningService>();

        return services;
    }
}
