using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Wedstrijd;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/wedstrijden")]
[Authorize]
public class WedstrijdenController : ControllerBase
{
    private static readonly TimeSpan StandaardDuur = TimeSpan.FromMinutes(120);

    private readonly IWedstrijdRepository _wedstrijden;
    private readonly ITeamRepository _teams;
    private readonly ISpelerRepository _spelers;
    private readonly ITerreinRepository _terreinen;
    private readonly IPlanningService _planning;

    public WedstrijdenController(
        IWedstrijdRepository wedstrijden,
        ITeamRepository teams,
        ISpelerRepository spelers,
        ITerreinRepository terreinen,
        IPlanningService planning)
    {
        _wedstrijden = wedstrijden;
        _teams = teams;
        _spelers = spelers;
        _terreinen = terreinen;
        _planning = planning;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WedstrijdResponse>>> Lijst([FromQuery] int teamId)
    {
        if (!await MagTeamLezenAsync(teamId))
            return Forbid();

        var wedstrijden = await _wedstrijden.GetByTeamIdAsync(teamId);
        return Ok(wedstrijden.Select(MapResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WedstrijdDetailResponse>> Get(int id)
    {
        var wedstrijd = await _wedstrijden.GetMetDetailsAsync(id);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {id} bestaat niet.");

        if (!await MagTeamLezenAsync(wedstrijd.TeamId))
            return Forbid();

        return Ok(new WedstrijdDetailResponse
        {
            WedstrijdId = wedstrijd.WedstrijdId,
            Datum = wedstrijd.Datum,
            StartTijd = wedstrijd.StartTijd,
            Tegenstander = wedstrijd.Tegenstander,
            ThuisScore = wedstrijd.ThuisScore,
            UitScore = wedstrijd.UitScore,
            TeamId = wedstrijd.TeamId,
            TerreinId = wedstrijd.TerreinId,
            Status = wedstrijd.Status,
            Opmerking = wedstrijd.Opmerking,
            TeamNaam = wedstrijd.Team?.Naam ?? string.Empty,
            TerreinNaam = wedstrijd.Terrein?.Naam ?? string.Empty,
            HeeftOpstelling = wedstrijd.Opstelling is not null
        });
    }

    [HttpPost]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<WedstrijdResponse>> Maak([FromBody] MaakWedstrijdRequest req)
    {
        var team = await _teams.GetByIdAsync(req.TeamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {req.TeamId} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (!await _terreinen.ExistsAsync(req.TerreinId))
            throw new NietGevondenException($"Terrein met id {req.TerreinId} bestaat niet.");

        await _planning.CheckOfThrowAsync(req.TerreinId, req.Datum, req.StartTijd,
            req.StartTijd + StandaardDuur);

        var wedstrijd = new Wedstrijd
        {
            Datum = req.Datum,
            StartTijd = req.StartTijd,
            Tegenstander = req.Tegenstander,
            TeamId = req.TeamId,
            TerreinId = req.TerreinId,
            Status = WedstrijdStatus.Gepland
        };

        await _wedstrijden.AddAsync(wedstrijd);
        await _wedstrijden.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = wedstrijd.WedstrijdId }, MapResponse(wedstrijd));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<WedstrijdResponse>> WerkBij(int id, [FromBody] WerkWedstrijdBijRequest req)
    {
        var wedstrijd = await _wedstrijden.GetByIdAsync(id);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {id} bestaat niet.");

        var team = await _teams.GetByIdAsync(wedstrijd.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        var nieuweDatum = req.Datum ?? wedstrijd.Datum;
        var nieuweStart = req.StartTijd ?? wedstrijd.StartTijd;
        var nieuwTerrein = req.TerreinId ?? wedstrijd.TerreinId;

        var planningWijzigt = req.Datum.HasValue || req.StartTijd.HasValue || req.TerreinId.HasValue;
        if (planningWijzigt)
        {
            if (req.TerreinId.HasValue && !await _terreinen.ExistsAsync(req.TerreinId.Value))
                throw new NietGevondenException($"Terrein met id {req.TerreinId} bestaat niet.");

            await _planning.CheckOfThrowAsync(nieuwTerrein, nieuweDatum, nieuweStart,
                nieuweStart + StandaardDuur, negeerWedstrijdId: id);
        }

        if (req.Datum.HasValue) wedstrijd.Datum = req.Datum.Value;
        if (req.StartTijd.HasValue) wedstrijd.StartTijd = req.StartTijd.Value;
        if (req.Tegenstander is not null) wedstrijd.Tegenstander = req.Tegenstander;
        if (req.TerreinId.HasValue) wedstrijd.TerreinId = req.TerreinId.Value;
        if (req.ThuisScore.HasValue) wedstrijd.ThuisScore = req.ThuisScore;
        if (req.UitScore.HasValue) wedstrijd.UitScore = req.UitScore;
        if (req.Status.HasValue) wedstrijd.Status = req.Status.Value;
        if (req.Opmerking is not null) wedstrijd.Opmerking = req.Opmerking;

        _wedstrijden.Update(wedstrijd);
        await _wedstrijden.SaveChangesAsync();

        return Ok(MapResponse(wedstrijd));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<IActionResult> Annuleer(int id)
    {
        var wedstrijd = await _wedstrijden.GetByIdAsync(id);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {id} bestaat niet.");

        var team = await _teams.GetByIdAsync(wedstrijd.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        wedstrijd.Status = WedstrijdStatus.Geannuleerd;
        _wedstrijden.Update(wedstrijd);
        await _wedstrijden.SaveChangesAsync();

        return NoContent();
    }

    private static WedstrijdResponse MapResponse(Wedstrijd w) => new()
    {
        WedstrijdId = w.WedstrijdId,
        Datum = w.Datum,
        StartTijd = w.StartTijd,
        Tegenstander = w.Tegenstander,
        ThuisScore = w.ThuisScore,
        UitScore = w.UitScore,
        TeamId = w.TeamId,
        TerreinId = w.TerreinId,
        Status = w.Status,
        Opmerking = w.Opmerking
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
