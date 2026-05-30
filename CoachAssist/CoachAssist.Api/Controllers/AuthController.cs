using System.Security.Claims;
using CoachAssist.Api.Services;
using CoachAssist.Core.DTOs.Auth;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly AppDbContext _db;

    public AuthController(AuthService auth, AppDbContext db)
    {
        _auth = auth;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
    {
        var resultaat = await _auth.RegisterAsync(req);
        if (resultaat is null)
            return Conflict(new { melding = "Er bestaat al een gebruiker met dit e-mailadres." });

        return Ok(resultaat);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var resultaat = await _auth.LoginAsync(req);
        if (resultaat is null)
            return Unauthorized(new { melding = "E-mail of wachtwoord is onjuist." });

        return Ok(resultaat);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> Me()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var id))
            return Unauthorized();

        var gebruiker = await _db.Gebruikers
            .Where(g => g.GebruikerId == id)
            .Select(g => new CurrentUserResponse
            {
                GebruikerId = g.GebruikerId,
                Email = g.Email,
                Voornaam = g.Voornaam,
                Achternaam = g.Achternaam,
                Rol = g.Rol,
            })
            .FirstOrDefaultAsync();

        if (gebruiker is null) return NotFound();
        return Ok(gebruiker);
    }
}
