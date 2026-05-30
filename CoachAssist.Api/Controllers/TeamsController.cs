using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Api.Services;
using CoachAssist.Core.DTOs.Statistiek;
using CoachAssist.Core.DTOs.Speler;
using CoachAssist.Core.DTOs.Team;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ITeamRepository _teams;
    private readonly ISpelerRepository _spelers;
    private readonly StatistiekService _statistiekService;

    public TeamsController(
        ITeamRepository teams,
        ISpelerRepository spelers,
        StatistiekService statistiekService)
    {
        _teams = teams;
        _spelers = spelers;
        _statistiekService = statistiekService;
    }

    [HttpGet("me")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TeamDetailResponse>> GetMijnTeam()
    {
        var gebruikerId = User.GetGebruikerId();
        var teams = await _teams.GetByCoachIdAsync(gebruikerId);
        var team = teams.OrderBy(t => t.Naam).FirstOrDefault();
        if (team is null)
            throw new NietGevondenException("Er is nog geen team gekoppeld aan deze coach.");

        var detail = await _teams.GetMetSpelersAsync(team.TeamId);
        if (detail is null)
            throw new NietGevondenException($"Team met id {team.TeamId} bestaat niet.");

        return Ok(MapDetail(detail));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamDetailResponse>> Get(int id)
    {
        if (!await MagTeamLezenAsync(id))
            return Forbid();

        var team = await _teams.GetMetSpelersAsync(id);
        if (team is null)
            throw new NietGevondenException($"Team met id {id} bestaat niet.");

        return Ok(MapDetail(team));
    }

    [HttpGet("{id:int}/spelers")]
    public async Task<ActionResult<List<SpelerResponse>>> GetSpelers(int id)
    {
        if (!await MagTeamLezenAsync(id))
            return Forbid();

        var spelers = await _spelers.GetByTeamIdAsync(id);
        var response = spelers.Select(s => new SpelerResponse
        {
            SpelerId = s.SpelerId,
            Voornaam = s.Voornaam,
            Achternaam = s.Achternaam,
            Rugnummer = s.Rugnummer,
            VastePositie = s.VastePositie,
            TeamId = s.TeamId
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}/seizoen")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<SeizoenTeamOverzicht>> GetSeizoen(int id)
    {
        var team = await _teams.GetByIdAsync(id);
        if (team is null)
            throw new NietGevondenException($"Team met id {id} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        return Ok(await _statistiekService.GetSeizoenOverzichtAsync(id));
    }

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

    private static TeamDetailResponse MapDetail(Core.Entities.Team team) => new()
    {
        TeamId = team.TeamId,
        Naam = team.Naam,
        Seizoen = team.Seizoen,
        ClubId = team.ClubId,
        CoachId = team.CoachId,
        CoachNaam = team.Coach is null ? null : $"{team.Coach.Voornaam} {team.Coach.Achternaam}",
        ClubNaam = team.Club?.Naam ?? string.Empty,
        Spelers = team.Spelers.Select(s => new SpelerResponse
        {
            SpelerId = s.SpelerId,
            Voornaam = s.Voornaam,
            Achternaam = s.Achternaam,
            Rugnummer = s.Rugnummer,
            VastePositie = s.VastePositie,
            TeamId = s.TeamId
        }).ToList()
    };
}
