using Microsoft.Extensions.DependencyInjection;

namespace CoachAssist.Shared.Services;

public static class CoachAssistApiServiceCollectionExtensions
{
    public const string DefaultApiBaseUrl = "http://localhost:5185";

    public static IServiceCollection AddCoachAssistApiClient(
        this IServiceCollection services,
        string? apiBaseUrl = null)
    {
        services.AddScoped<IAuthSession, AuthSession>();
        services.AddScoped<RegistrationDraftService>();
        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(NormalizeBaseUrl(apiBaseUrl)),
        });
        services.AddScoped<ICoachAssistApiClient, CoachAssistApiClient>();
        services.AddScoped<CoachAssistDataContext>();

        return services;
    }

    private static string NormalizeBaseUrl(string? apiBaseUrl)
    {
        var value = string.IsNullOrWhiteSpace(apiBaseUrl)
            ? DefaultApiBaseUrl
            : apiBaseUrl.Trim();

        return value.EndsWith("/", StringComparison.Ordinal)
            ? value
            : value + "/";
    }
}
