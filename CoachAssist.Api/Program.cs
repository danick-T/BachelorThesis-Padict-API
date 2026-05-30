using System.Text;
using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Api.Services;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.NameTranslation;

var builder = WebApplication.CreateBuilder(args);
var postgresEnumNameTranslator = new NpgsqlNullNameTranslator();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSecretFromEnvironment = Environment.GetEnvironmentVariable("COACHASSIST_JWT_SECRET");

if (!string.IsNullOrWhiteSpace(jwtSecretFromEnvironment))
    builder.Configuration["JwtSettings:SecretKey"] = jwtSecretFromEnvironment;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// OpenAPI / Swagger
builder.Services.AddOpenApi();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "data-protection-keys")));

// EF Core + PostgreSQL (Neon). De Web- en MAUI-app raken de databank
// nooit rechtstreeks aan — enkel deze API doet dat.
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection ontbreekt. Zet deze lokaal via user secrets " +
        "of via de environment variable ConnectionStrings__DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsql =>
        {
            npgsql.MapEnum<GebruikerRol>("gebruikersrol", nameTranslator: postgresEnumNameTranslator);
            npgsql.MapEnum<AanvraagStatus>("aanvraagstatus", nameTranslator: postgresEnumNameTranslator);
            npgsql.MapEnum<WedstrijdStatus>("wedstrijdstatus", nameTranslator: postgresEnumNameTranslator);
            npgsql.MapEnum<TrainingStatus>("trainingstatus", nameTranslator: postgresEnumNameTranslator);
            npgsql.MapEnum<VastePositie>("vaste_positie", nameTranslator: postgresEnumNameTranslator);
            npgsql.MapEnum<OpstellingPositie>("opstelling_positie", nameTranslator: postgresEnumNameTranslator);
        }));

// JWT-configuratie inlezen
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings ontbreekt in configuratie.");

if (!string.IsNullOrWhiteSpace(jwtSecretFromEnvironment))
    jwtSettings.SecretKey = jwtSecretFromEnvironment;

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException(
        "JwtSettings:SecretKey ontbreekt. Zet deze in appsettings.Development.json (lokaal) " +
        "of in de environment variable COACHASSIST_JWT_SECRET (productie).");

// Password hasher voor Gebruiker (we gebruiken enkel deze helper uit Identity,
// niet de volledige IdentityDbContext — schema blijft zo intact).
builder.Services.AddSingleton<IPasswordHasher<Gebruiker>, PasswordHasher<Gebruiker>>();
builder.Services.AddScoped<AuthService>();

// JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization(opt => opt.VoegCoachAssistPoliciesToe());

// Repositories en domein-services (slice-agents vullen aan)
builder.Services.AddRepositories();

// CORS — laat de Web- en MAUI-host de API aanspreken
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapControllers();

app.Run();
