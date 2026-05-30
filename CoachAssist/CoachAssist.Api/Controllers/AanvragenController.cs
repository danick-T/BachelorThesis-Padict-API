using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Aanvraag;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

// Bij een TeamAanvraag maken we indien nodig een basis Speler-record aan.
// De tabel vereist vandaag al een TeamId, dus het gekozen team wordt meteen gekoppeld.
// De aanvraagstatus blijft wel bepalen of de speler door de coach is goedgekeurd.
[ApiController]
[Route("api/aanvragen")]
[Authorize]
public class AanvragenController : ControllerBase
{
    private readonly IAanvraagRepository _aanvragen;
    private readonly AppDbContext _db;

    public AanvragenController(IAanvraagRepository aanvragen, AppDbContext db)
    {
        _aanvragen = aanvragen;
        _db = db;
    }

    // ---------------- Club ----------------

    [HttpPost("club")]
    [Authorize(Policy = Policies.SpelerOnly)]
    public async Task<ActionResult<ClubAanvraagResponse>> MaakClubAanvraag([FromBody] MaakClubAanvraagRequest req)
    {
        var gebruikerId = User.GetGebruikerId();

        var club = await _db.Clubs.FirstOrDefaultAsync(c => c.ClubId == req.ClubId);
        if (club is null)
            throw new NietGevondenException($"Club met id {req.ClubId} bestaat niet.");

        var bestaand = await _aanvragen.ZoekClubAanvraagAsync(gebruikerId, req.ClubId);
        if (bestaand is not null)
            return Conflict(new { melding = "Je hebt al een aanvraag gedaan bij deze club." });

        var gebruiker = await _db.Gebruikers.FirstOrDefaultAsync(g => g.GebruikerId == gebruikerId);
        if (gebruiker is null)
            throw new NietGevondenException("Ingelogde gebruiker niet gevonden.");

        var aanvraag = new ClubAanvraag
        {
            GebruikerId = gebruikerId,
            ClubId = req.ClubId,
            Status = AanvraagStatus.InAfwachting,
            DatumAanvraag = DateTime.UtcNow
        };

        await _aanvragen.AddClubAanvraagAsync(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(new ClubAanvraagResponse
        {
            AanvraagId = aanvraag.AanvraagId,
            GebruikerId = gebruikerId,
            GebruikerNaam = $"{gebruiker.Voornaam} {gebruiker.Achternaam}",
            Email = gebruiker.Email,
            ClubId = club.ClubId,
            ClubNaam = club.Naam,
            Status = aanvraag.Status,
            DatumAanvraag = aanvraag.DatumAanvraag
        });
    }

    [HttpGet("club")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<List<ClubAanvraagResponse>>> GetOpenstaandeClubAanvragen([FromQuery] int clubId)
    {
        var lijst = await _aanvragen.GetOpenstaandeClubAanvragenAsync(clubId);
        var response = lijst.Select(a => new ClubAanvraagResponse
        {
            AanvraagId = a.AanvraagId,
            GebruikerId = a.GebruikerId,
            GebruikerNaam = a.Gebruiker is null ? string.Empty : $"{a.Gebruiker.Voornaam} {a.Gebruiker.Achternaam}",
            Email = a.Gebruiker?.Email ?? string.Empty,
            ClubId = a.ClubId,
            ClubNaam = a.Club?.Naam ?? string.Empty,
            Status = a.Status,
            DatumAanvraag = a.DatumAanvraag
        }).ToList();

        return Ok(response);
    }

    [HttpPut("club/{id:int}/goedkeuren")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<ClubAanvraagResponse>> GoedkeurenClub(int id)
    {
        var aanvraag = await _aanvragen.GetClubAanvraagAsync(id);
        if (aanvraag is null)
            throw new NietGevondenException($"ClubAanvraag met id {id} bestaat niet.");

        if (aanvraag.Status != AanvraagStatus.InAfwachting)
            throw new DomeinFoutException("Deze aanvraag is al verwerkt.");

        aanvraag.Status = AanvraagStatus.Goedgekeurd;
        _aanvragen.UpdateClub(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(MaakClubResponse(aanvraag));
    }

    [HttpPut("club/{id:int}/weigeren")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<ClubAanvraagResponse>> WeigerenClub(int id)
    {
        var aanvraag = await _aanvragen.GetClubAanvraagAsync(id);
        if (aanvraag is null)
            throw new NietGevondenException($"ClubAanvraag met id {id} bestaat niet.");

        if (aanvraag.Status != AanvraagStatus.InAfwachting)
            throw new DomeinFoutException("Deze aanvraag is al verwerkt.");

        aanvraag.Status = AanvraagStatus.Geweigerd;
        _aanvragen.UpdateClub(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(MaakClubResponse(aanvraag));
    }

    // ---------------- Team ----------------

    [HttpPost("team")]
    [Authorize(Policy = Policies.SpelerOnly)]
    public async Task<ActionResult<TeamAanvraagResponse>> MaakTeamAanvraag([FromBody] MaakTeamAanvraagRequest req)
    {
        var gebruikerId = User.GetGebruikerId();

        var team = await _db.Teams.FirstOrDefaultAsync(t => t.TeamId == req.TeamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {req.TeamId} bestaat niet.");

        var heeftClubGoedkeuring = await _aanvragen.HeeftGoedgekeurdeClubAanvraagAsync(gebruikerId, team.ClubId);
        if (!heeftClubGoedkeuring)
            throw new DomeinFoutException("Je hebt eerst een goedgekeurde lidmaatschap nodig bij deze club.");

        var speler = await _db.Spelers.FirstOrDefaultAsync(s => s.GebruikerId == gebruikerId);
        if (speler is null)
        {
            var gebruiker = await _db.Gebruikers.FirstOrDefaultAsync(g => g.GebruikerId == gebruikerId);
            if (gebruiker is null)
                throw new NietGevondenException("Ingelogde gebruiker niet gevonden.");

            speler = new Speler
            {
                Voornaam = gebruiker.Voornaam,
                Achternaam = gebruiker.Achternaam,
                TeamId = req.TeamId,
                GebruikerId = gebruikerId,
                VastePositie = VastePositie.NietGespecificeerd
            };
            _db.Spelers.Add(speler);
            await _db.SaveChangesAsync();
        }

        var bestaand = await _aanvragen.ZoekTeamAanvraagAsync(speler.SpelerId, req.TeamId);
        if (bestaand is not null)
            return Conflict(new { melding = "Je hebt al een aanvraag gedaan bij dit team." });

        var aanvraag = new TeamAanvraag
        {
            SpelerId = speler.SpelerId,
            TeamId = req.TeamId,
            Status = AanvraagStatus.InAfwachting,
            DatumAanvraag = DateTime.UtcNow
        };

        await _aanvragen.AddTeamAanvraagAsync(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(new TeamAanvraagResponse
        {
            AanvraagId = aanvraag.AanvraagId,
            SpelerId = speler.SpelerId,
            SpelerNaam = $"{speler.Voornaam} {speler.Achternaam}",
            TeamId = team.TeamId,
            TeamNaam = team.Naam,
            Status = aanvraag.Status,
            DatumAanvraag = aanvraag.DatumAanvraag
        });
    }

    [HttpGet("team")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<List<TeamAanvraagResponse>>> GetOpenstaandeTeamAanvragen([FromQuery] int teamId)
    {
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {teamId} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        var lijst = await _aanvragen.GetOpenstaandeTeamAanvragenAsync(teamId);
        var response = lijst.Select(a => new TeamAanvraagResponse
        {
            AanvraagId = a.AanvraagId,
            SpelerId = a.SpelerId,
            SpelerNaam = a.Speler is null ? string.Empty : $"{a.Speler.Voornaam} {a.Speler.Achternaam}",
            TeamId = a.TeamId,
            TeamNaam = a.Team?.Naam ?? string.Empty,
            Status = a.Status,
            DatumAanvraag = a.DatumAanvraag
        }).ToList();

        return Ok(response);
    }

    [HttpPut("team/{id:int}/goedkeuren")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TeamAanvraagResponse>> GoedkeurenTeam(int id)
    {
        var aanvraag = await _aanvragen.GetTeamAanvraagAsync(id);
        if (aanvraag is null)
            throw new NietGevondenException($"TeamAanvraag met id {id} bestaat niet.");

        if (aanvraag.Team is null || aanvraag.Speler is null)
            throw new NietGevondenException("Bijhorende speler of team niet gevonden.");

        if (aanvraag.Team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (aanvraag.Status != AanvraagStatus.InAfwachting)
            throw new DomeinFoutException("Deze aanvraag is al verwerkt.");

        aanvraag.Status = AanvraagStatus.Goedgekeurd;
        aanvraag.Speler.TeamId = aanvraag.TeamId;

        _aanvragen.UpdateTeam(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(MaakTeamResponse(aanvraag));
    }

    [HttpPut("team/{id:int}/weigeren")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TeamAanvraagResponse>> WeigerenTeam(int id)
    {
        var aanvraag = await _aanvragen.GetTeamAanvraagAsync(id);
        if (aanvraag is null)
            throw new NietGevondenException($"TeamAanvraag met id {id} bestaat niet.");

        if (aanvraag.Team is null)
            throw new NietGevondenException("Bijhorend team niet gevonden.");

        if (aanvraag.Team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (aanvraag.Status != AanvraagStatus.InAfwachting)
            throw new DomeinFoutException("Deze aanvraag is al verwerkt.");

        aanvraag.Status = AanvraagStatus.Geweigerd;
        _aanvragen.UpdateTeam(aanvraag);
        await _aanvragen.SaveChangesAsync();

        return Ok(MaakTeamResponse(aanvraag));
    }

    // ---------------- Helpers ----------------

    private static ClubAanvraagResponse MaakClubResponse(ClubAanvraag a) => new()
    {
        AanvraagId = a.AanvraagId,
        GebruikerId = a.GebruikerId,
        GebruikerNaam = a.Gebruiker is null ? string.Empty : $"{a.Gebruiker.Voornaam} {a.Gebruiker.Achternaam}",
        Email = a.Gebruiker?.Email ?? string.Empty,
        ClubId = a.ClubId,
        ClubNaam = a.Club?.Naam ?? string.Empty,
        Status = a.Status,
        DatumAanvraag = a.DatumAanvraag
    };

    private static TeamAanvraagResponse MaakTeamResponse(TeamAanvraag a) => new()
    {
        AanvraagId = a.AanvraagId,
        SpelerId = a.SpelerId,
        SpelerNaam = a.Speler is null ? string.Empty : $"{a.Speler.Voornaam} {a.Speler.Achternaam}",
        TeamId = a.TeamId,
        TeamNaam = a.Team?.Naam ?? string.Empty,
        Status = a.Status,
        DatumAanvraag = a.DatumAanvraag
    };
}
