using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CoachAssist.Core.DTOs.Auth;

namespace CoachAssist.Shared.Services;

public sealed class CoachAssistApiClient : ICoachAssistApiClient
{
    private readonly HttpClient _http;

    public CoachAssistApiClient(HttpClient http, IAuthSession authSession)
    {
        _http = http;
        AuthSession = authSession;
    }

    public IAuthSession AuthSession { get; }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var auth = await PostForJsonAsync<AuthResponse>("api/auth/login", request, cancellationToken);
        AuthSession.SetAuth(auth);
        return auth;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var auth = await PostForJsonAsync<AuthResponse>("api/auth/register", request, cancellationToken);
        AuthSession.SetAuth(auth);
        return auth;
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
        using var response = await SendAuthorizedAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response, cancellationToken);
        var gebruiker = await response.Content.ReadFromJsonAsync<CurrentUserResponse>(cancellationToken);

        if (gebruiker is not null)
        {
            AuthSession.SetGebruiker(gebruiker);
        }

        return gebruiker;
    }

    public async Task<T?> GetFromJsonAuthorizedAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        using var response = await SendAuthorizedAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsJsonAuthorizedAsync<T>(
        string uri,
        T value,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(value),
        };

        return SendAuthorizedAsync(request, cancellationToken);
    }

    public Task<HttpResponseMessage> SendAuthorizedAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(AuthSession.Token))
        {
            throw new AuthApiException("Je bent niet aangemeld.", 401);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthSession.Token);
        return _http.SendAsync(request, cancellationToken);
    }

    public void Logout()
    {
        AuthSession.Clear();
    }

    private async Task<T> PostForJsonAsync<T>(
        string uri,
        object value,
        CancellationToken cancellationToken)
    {
        using var response = await _http.PostAsJsonAsync(uri, value, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        if (result is null)
        {
            throw new AuthApiException("De server gaf geen geldig antwoord terug.", (int)response.StatusCode);
        }

        return result;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await ReadErrorMessageAsync(response, cancellationToken);
        throw new AuthApiException(message, (int)response.StatusCode);
    }

    private static async Task<string> ReadErrorMessageAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var fallback = response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "E-mail of wachtwoord is onjuist.",
            System.Net.HttpStatusCode.Conflict => "Er bestaat al een gebruiker met dit e-mailadres.",
            _ => "De aanvraag kon niet verwerkt worden.",
        };

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return fallback;
        }

        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("melding", out var melding))
            {
                return melding.GetString() ?? fallback;
            }

            if (json.RootElement.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? fallback;
            }
        }
        catch (JsonException)
        {
            return body;
        }

        return fallback;
    }
}
