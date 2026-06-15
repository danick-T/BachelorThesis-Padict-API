using CoachAssist.Core.DTOs.Auth;

namespace CoachAssist.Shared.Services;

public interface ICoachAssistApiClient
{
    IAuthSession AuthSession { get; }

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<CurrentUserResponse?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAuthorizedAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<T?> GetFromJsonAuthorizedAsync<T>(string uri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsJsonAuthorizedAsync<T>(string uri, T value, CancellationToken cancellationToken = default);
    Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
    void Logout();
}
