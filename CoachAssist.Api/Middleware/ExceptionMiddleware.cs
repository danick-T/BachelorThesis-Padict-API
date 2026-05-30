using System.Text.Json;

namespace CoachAssist.Api.Middleware;

public class PlanningConflictException : Exception
{
    public PlanningConflictException(string melding) : base(melding) { }
}

public class NietGevondenException : Exception
{
    public NietGevondenException(string melding) : base(melding) { }
}

public class DomeinFoutException : Exception
{
    public DomeinFoutException(string melding) : base(melding) { }
}

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _log;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> log, IHostEnvironment env)
    {
        _next = next;
        _log = log;
        _env = env;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (PlanningConflictException ex)
        {
            await SchrijfFout(ctx, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (NietGevondenException ex)
        {
            await SchrijfFout(ctx, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomeinFoutException ex)
        {
            await SchrijfFout(ctx, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Onverwachte fout in API");
            await SchrijfFout(ctx, StatusCodes.Status500InternalServerError,
                "Er is een onverwachte fout opgetreden.",
                _env.IsDevelopment() ? ex.ToString() : null);
        }
    }

    private static async Task SchrijfFout(HttpContext ctx, int status, string melding, string? details = null)
    {
        if (ctx.Response.HasStarted) return;
        ctx.Response.Clear();
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json";
        var body = details is null
            ? JsonSerializer.Serialize(new { melding })
            : JsonSerializer.Serialize(new { melding, details });
        await ctx.Response.WriteAsync(body);
    }
}
