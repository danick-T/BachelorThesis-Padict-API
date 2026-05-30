namespace CoachAssist.Shared.Services;

public sealed class AuthApiException : Exception
{
    public AuthApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
