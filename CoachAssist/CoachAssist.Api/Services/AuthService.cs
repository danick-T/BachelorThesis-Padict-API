using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoachAssist.Core.DTOs.Auth;
using CoachAssist.Core.Entities;
using CoachAssist.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoachAssist.Api.Services;

// Fase 1: minimale auth bovenop het bestaande Gebruiker-schema.
// We gebruiken enkel PasswordHasher uit Identity, niet de volledige IdentityDbContext.
// Upgraden naar IdentityUser<int> kan later wanneer een schema-migratie op Neon kan.
public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings _jwt;
    private readonly PasswordHasher<Gebruiker> _hasher = new();

    public AuthService(AppDbContext db, IOptions<JwtSettings> jwt)
    {
        _db = db;
        _jwt = jwt.Value;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest req)
    {
        var bestaat = await _db.Gebruikers.AnyAsync(g => g.Email == req.Email);
        if (bestaat) return null;

        var gebruiker = new Gebruiker
        {
            Email = req.Email,
            Voornaam = req.Voornaam,
            Achternaam = req.Achternaam,
            Rol = req.Rol,
        };
        gebruiker.PasswordHash = _hasher.HashPassword(gebruiker, req.Wachtwoord);

        _db.Gebruikers.Add(gebruiker);
        await _db.SaveChangesAsync();

        return BouwAuthResponse(gebruiker);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        var gebruiker = await _db.Gebruikers.FirstOrDefaultAsync(g => g.Email == req.Email);
        if (gebruiker is null) return null;

        var resultaat = _hasher.VerifyHashedPassword(gebruiker, gebruiker.PasswordHash, req.Wachtwoord);
        if (resultaat == PasswordVerificationResult.Failed) return null;

        if (resultaat == PasswordVerificationResult.SuccessRehashNeeded)
        {
            gebruiker.PasswordHash = _hasher.HashPassword(gebruiker, req.Wachtwoord);
            await _db.SaveChangesAsync();
        }

        return BouwAuthResponse(gebruiker);
    }

    private AuthResponse BouwAuthResponse(Gebruiker gebruiker)
    {
        var verlooptOp = DateTime.UtcNow.AddDays(_jwt.ExpiryInDays);
        var token = GenereerToken(gebruiker, verlooptOp);

        return new AuthResponse
        {
            Token = token,
            VerlooptOp = verlooptOp,
            Gebruiker = new CurrentUserResponse
            {
                GebruikerId = gebruiker.GebruikerId,
                Email = gebruiker.Email,
                Voornaam = gebruiker.Voornaam,
                Achternaam = gebruiker.Achternaam,
                Rol = gebruiker.Rol,
            },
        };
    }

    private string GenereerToken(Gebruiker gebruiker, DateTime verlooptOp)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, gebruiker.GebruikerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, gebruiker.Email),
            new Claim(ClaimTypes.NameIdentifier, gebruiker.GebruikerId.ToString()),
            new Claim(ClaimTypes.Role, gebruiker.Rol.ToString()),
            new Claim("voornaam", gebruiker.Voornaam),
            new Claim("achternaam", gebruiker.Achternaam),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: verlooptOp,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
