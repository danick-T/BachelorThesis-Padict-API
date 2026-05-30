using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Statistiek;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/statistieken")]
public class StatistiekenController : ControllerBase
{
    private readonly IStatistiekRepository _stats;
    private readonly AppDbContext _db;
    private readonly ITeamRepository _teams;
    private readonly ISpelerRepository _spelers;

    public StatistiekenController(
        IStatistiekRepository stats,
        AppDbContext db,
        ITeamRepository teams,
        ISpelerRepository spelers)
    {
        _stats = stats;
        _db = db;
        _teams = teams;
        _spelers = spelers;
    }

    [HttpPost("wedstrijd/{wedstrijdId:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<List<StatistiekResponse>>> BulkInvoeren(
        int wedstrijdId, [FromBody] BulkStatistiekenRequest req)
    {
        var wedstrijd = await _db.Wedstrijden
            .Include(w => w.Team)
            .FirstOrDefaultAsync(w => w.WedstrijdId == wedstrijdId);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {wedstrijdId} bestaat niet.");

        if (wedstrijd.Team.CoachId != User.GetGebruikerId())
            return Forbid();

        foreach (var dto in req.Statistieken)
        {
            var bestaand = await _stats.ZoekAsync(dto.SpelerId, wedstrijdId);
            if (bestaand is null)
            {
                await _stats.AddAsync(new SpelerStatistiek
                {
                    SpelerId = dto.SpelerId,
                    WedstrijdId = wedstrijdId,
                    Goals = dto.Goals,
                    Assists = dto.Assists,
                    GeleKaarten = dto.GeleKaarten,
                    RodeKaarten = dto.RodeKaarten,
                    GespeeldeMinuten = dto.GespeeldeMinuten,
                });
            }
            else
            {
                bestaand.Goals = dto.Goals;
                bestaand.Assists = dto.Assists;
                bestaand.GeleKaarten = dto.GeleKaarten;
                bestaand.RodeKaarten = dto.RodeKaarten;
                bestaand.GespeeldeMinuten = dto.GespeeldeMinuten;
                _stats.Update(bestaand);
            }
        }

        if (req.ThuisScore is not null && req.UitScore is not null)
        {
            wedstrijd.ThuisScore = req.ThuisScore;
            wedstrijd.UitScore = req.UitScore;
            wedstrijd.Status = WedstrijdStatus.Gespeeld;
        }

        await _stats.SaveChangesAsync();

        var alles = await _stats.GetByWedstrijdIdAsync(wedstrijdId);
        return Ok(alles.Select(MapNaarResponse).ToList());
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<StatistiekResponse>> WerkBij(int id, [FromBody] SpelerStatistiekDto dto)
    {
        var stat = await _stats.GetByIdAsync(id);
        if (stat is null) throw new NietGevondenException($"Statistiek met id {id} bestaat niet.");

        var wedstrijd = await _db.Wedstrijden
            .Include(w => w.Team)
            .FirstAsync(w => w.WedstrijdId == stat.WedstrijdId);
        if (wedstrijd.Team.CoachId != User.GetGebruikerId())
            return Forbid();

        stat.Goals = dto.Goals;
        stat.Assists = dto.Assists;
        stat.GeleKaarten = dto.GeleKaarten;
        stat.RodeKaarten = dto.RodeKaarten;
        stat.GespeeldeMinuten = dto.GespeeldeMinuten;
        _stats.Update(stat);
        await _stats.SaveChangesAsync();

        var herladen = await _db.SpelerStatistieken
            .Include(s => s.Speler)
            .FirstAsync(s => s.StatistiekId == id);
        return Ok(MapNaarResponse(herladen));
    }

    [HttpGet("wedstrijd/{wedstrijdId:int}")]
    [Authorize]
    public async Task<ActionResult<List<StatistiekResponse>>> GetVoorWedstrijd(int wedstrijdId)
    {
        var wedstrijd = await _db.Wedstrijden.AsNoTracking()
            .FirstOrDefaultAsync(w => w.WedstrijdId == wedstrijdId);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {wedstrijdId} bestaat niet.");

        if (!await MagTeamLezenAsync(wedstrijd.TeamId))
            return Forbid();

        var stats = await _stats.GetByWedstrijdIdAsync(wedstrijdId);
        return Ok(stats.Select(MapNaarResponse).ToList());
    }

    private static StatistiekResponse MapNaarResponse(SpelerStatistiek s) => new()
    {
        StatistiekId = s.StatistiekId,
        SpelerId = s.SpelerId,
        SpelerNaam = s.Speler is null ? string.Empty : $"{s.Speler.Voornaam} {s.Speler.Achternaam}",
        WedstrijdId = s.WedstrijdId,
        Goals = s.Goals,
        Assists = s.Assists,
        GeleKaarten = s.GeleKaarten,
        RodeKaarten = s.RodeKaarten,
        GespeeldeMinuten = s.GespeeldeMinuten,
    };

    private async Task<bool> MagTeamLezenAsync(int teamId)
    {
        var team = await _teams.GetByIdAsync(teamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {teamId} bestaat niet.");

        var gebruikerId = User.GetGebruikerId();
        if (User.IsInRol(GebruikerRol.Admin) || team.CoachId == gebruikerId)
        {
            return true;
        }

        if (!User.IsInRol(GebruikerRol.Speler))
        {
            return false;
        }

        var speler = await _spelers.GetByGebruikerIdAsync(gebruikerId);
        return speler?.TeamId == teamId;
    }
}
