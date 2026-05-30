using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Api.Services;
using CoachAssist.Core.DTOs.Statistiek;
using CoachAssist.Core.DTOs.Speler;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/spelers")]
[Authorize]
public class SpelersController : ControllerBase
{
    private readonly ISpelerRepository _spelers;
    private readonly ITeamRepository _teams;
    private readonly IStatistiekRepository _stats;
    private readonly StatistiekService _statistiekService;
    private readonly AppDbContext _db;

    public SpelersController(
        ISpelerRepository spelers,
        ITeamRepository teams,
        IStatistiekRepository stats,
        StatistiekService statistiekService,
        AppDbContext db)
    {
        _spelers = spelers;
        _teams = teams;
        _stats = stats;
        _statistiekService = statistiekService;
        _db = db;
    }

    [HttpGet("me")]
    [Authorize(Policy = Policies.SpelerOnly)]
    public async Task<ActionResult<SpelerDetailResponse>> GetMe()
    {
        var gebruikerId = User.GetGebruikerId();
        var speler = await _db.Spelers.AsNoTracking()
            .Include(s => s.Team)
                .ThenInclude(t => t.Club)
            .Include(s => s.Gebruiker)
            .FirstOrDefaultAsync(s => s.GebruikerId == gebruikerId);

        if (speler is null)
            throw new NietGevondenException("Er is nog geen spelerprofiel gekoppeld aan dit account.");

        return Ok(MapDetail(speler));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpelerDetailResponse>> Get(int id)
    {
        var speler = await _spelers.GetByIdAsync(id);
        if (speler is null)
            throw new NietGevondenException($"Speler met id {id} bestaat niet.");

        return Ok(MapDetail(speler));
    }

    [HttpPost]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<SpelerResponse>> Maak([FromBody] MaakSpelerRequest req)
    {
        var team = await _teams.GetByIdAsync(req.TeamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {req.TeamId} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        var speler = new Speler
        {
            Voornaam = req.Voornaam,
            Achternaam = req.Achternaam,
            Rugnummer = req.Rugnummer,
            VastePositie = req.VastePositie,
            TeamId = req.TeamId
        };

        await _spelers.AddAsync(speler);
        await _spelers.SaveChangesAsync();

        var response = MapResponse(speler);
        return CreatedAtAction(nameof(Get), new { id = speler.SpelerId }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<SpelerResponse>> WerkBij(int id, [FromBody] WerkSpelerBijRequest req)
    {
        var speler = await _spelers.GetByIdAsync(id);
        if (speler is null)
            throw new NietGevondenException($"Speler met id {id} bestaat niet.");

        var team = await _teams.GetByIdAsync(speler.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (req.Voornaam is not null) speler.Voornaam = req.Voornaam;
        if (req.Achternaam is not null) speler.Achternaam = req.Achternaam;
        if (req.Rugnummer is not null) speler.Rugnummer = req.Rugnummer;
        if (req.VastePositie is not null) speler.VastePositie = req.VastePositie;

        _spelers.Update(speler);
        await _spelers.SaveChangesAsync();

        return Ok(MapResponse(speler));
    }

    [HttpGet("{id:int}/stats")]
    public async Task<ActionResult<List<StatistiekResponse>>> GetStats(int id)
    {
        if (!await MagSpelerLezenAsync(id))
            return Forbid();

        var stats = await _stats.GetBySpelerIdAsync(id);
        return Ok(stats.Select(MapStatistiekResponse).ToList());
    }

    [HttpGet("{id:int}/seizoen")]
    public async Task<ActionResult<SeizoenSpelerStats>> GetSeizoen(int id)
    {
        if (!await MagSpelerLezenAsync(id))
            return Forbid();

        return Ok(await _statistiekService.GetSpelerSeizoenAsync(id));
    }

    private static SpelerResponse MapResponse(Speler s) => new()
    {
        SpelerId = s.SpelerId,
        Voornaam = s.Voornaam,
        Achternaam = s.Achternaam,
        Rugnummer = s.Rugnummer,
        VastePositie = s.VastePositie,
        TeamId = s.TeamId
    };

    private static SpelerDetailResponse MapDetail(Speler s) => new()
    {
        SpelerId = s.SpelerId,
        Voornaam = s.Voornaam,
        Achternaam = s.Achternaam,
        Rugnummer = s.Rugnummer,
        VastePositie = s.VastePositie,
        TeamId = s.TeamId,
        TeamNaam = s.Team?.Naam ?? string.Empty,
        ClubId = s.Team?.ClubId ?? 0,
        ClubNaam = s.Team?.Club?.Naam ?? string.Empty,
        GebruikerId = s.GebruikerId,
        Email = s.Gebruiker?.Email
    };

    private async Task<bool> MagSpelerLezenAsync(int spelerId)
    {
        var speler = await _db.Spelers.AsNoTracking()
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.SpelerId == spelerId);

        if (speler is null)
            throw new NietGevondenException($"Speler met id {spelerId} bestaat niet.");

        var gebruikerId = User.GetGebruikerId();
        return User.IsInRol(GebruikerRol.Admin)
               || speler.GebruikerId == gebruikerId
               || speler.Team?.CoachId == gebruikerId;
    }

    private static StatistiekResponse MapStatistiekResponse(SpelerStatistiek s) => new()
    {
        StatistiekId = s.StatistiekId,
        SpelerId = s.SpelerId,
        SpelerNaam = s.Speler is null ? string.Empty : $"{s.Speler.Voornaam} {s.Speler.Achternaam}".Trim(),
        WedstrijdId = s.WedstrijdId,
        Goals = s.Goals,
        Assists = s.Assists,
        GeleKaarten = s.GeleKaarten,
        RodeKaarten = s.RodeKaarten,
        GespeeldeMinuten = s.GespeeldeMinuten
    };
}
