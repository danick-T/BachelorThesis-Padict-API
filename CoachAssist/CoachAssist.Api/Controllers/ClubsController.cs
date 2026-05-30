using CoachAssist.Api.Authorization;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Club;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubEntity = CoachAssist.Core.Entities.Club;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/clubs")]
public class ClubsController : ControllerBase
{
    private readonly IClubRepository _clubs;
    private readonly AppDbContext _db;

    public ClubsController(IClubRepository clubs, AppDbContext db)
    {
        _clubs = clubs;
        _db = db;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClubResponse>>> Zoek([FromQuery] string? zoek)
    {
        var resultaat = await _clubs.ZoekAsync(zoek);
        var response = resultaat.Select(c => new ClubResponse
        {
            ClubId = c.ClubId,
            Naam = c.Naam,
            Stad = c.Stad,
        });
        return Ok(response);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost]
    public async Task<ActionResult<ClubResponse>> Maak([FromBody] MaakClubRequest req)
    {
        if (await _clubs.BestaatNaamAsync(req.Naam))
            return Conflict(new { melding = "Er bestaat al een club met deze naam." });

        var club = new ClubEntity { Naam = req.Naam, Stad = req.Stad };
        await _clubs.AddAsync(club);
        await _clubs.SaveChangesAsync();

        var response = new ClubResponse
        {
            ClubId = club.ClubId,
            Naam = club.Naam,
            Stad = club.Stad,
        };
        return CreatedAtAction(nameof(GetById), new { id = club.ClubId }, response);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClubDetailResponse>> GetById(int id)
    {
        var detail = await _db.Clubs.AsNoTracking()
            .Where(c => c.ClubId == id)
            .Select(c => new ClubDetailResponse
            {
                ClubId = c.ClubId,
                Naam = c.Naam,
                Stad = c.Stad,
                AantalTeams = c.Teams.Count,
                AantalTerreinen = c.Terreinen.Count,
            })
            .FirstOrDefaultAsync();

        if (detail is null)
            throw new NietGevondenException($"Club met id {id} bestaat niet.");

        return Ok(detail);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/teams")]
    public async Task<ActionResult<IEnumerable<object>>> GetTeams(int id)
    {
        if (!await _clubs.ExistsAsync(id))
            throw new NietGevondenException($"Club met id {id} bestaat niet.");

        var teams = await _db.Teams.AsNoTracking()
            .Where(t => t.ClubId == id)
            .OrderBy(t => t.Naam)
            .Select(t => new { t.TeamId, t.Naam, t.Seizoen })
            .ToListAsync();

        return Ok(teams);
    }
}
