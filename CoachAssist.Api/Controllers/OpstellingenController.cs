using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Opstelling;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/opstellingen")]
[Authorize]
public class OpstellingenController : ControllerBase
{
    private readonly IOpstellingRepository _opstellingen;
    private readonly IWedstrijdRepository _wedstrijden;
    private readonly ITeamRepository _teams;

    public OpstellingenController(
        IOpstellingRepository opstellingen,
        IWedstrijdRepository wedstrijden,
        ITeamRepository teams)
    {
        _opstellingen = opstellingen;
        _wedstrijden = wedstrijden;
        _teams = teams;
    }

    [HttpGet("{wedstrijdId:int}")]
    public async Task<ActionResult<OpstellingResponse>> Get(int wedstrijdId)
    {
        var opstelling = await _opstellingen.GetByWedstrijdIdAsync(wedstrijdId);
        if (opstelling is null)
            throw new NietGevondenException($"Geen opstelling gevonden voor wedstrijd {wedstrijdId}.");

        return Ok(Map(opstelling));
    }

    [HttpPost]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<OpstellingResponse>> Maak([FromBody] MaakOpstellingRequest req)
    {
        var wedstrijd = await _wedstrijden.GetByIdAsync(req.WedstrijdId);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {req.WedstrijdId} bestaat niet.");

        var team = await _teams.GetByIdAsync(wedstrijd.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        var bestaande = await _opstellingen.GetByWedstrijdIdAsync(req.WedstrijdId);
        if (bestaande is not null)
            throw new DomeinFoutException("Er bestaat al een opstelling voor deze wedstrijd.");

        var opstelling = new Opstelling
        {
            WedstrijdId = req.WedstrijdId,
            Formatie = req.Formatie,
            Spelers = req.Spelers.Select(s => new OpstellingSpeler
            {
                SpelerId = s.SpelerId,
                PositieNaam = s.PositieNaam,
                VeldPositieX = s.VeldPositieX,
                VeldPositieY = s.VeldPositieY
            }).ToList()
        };

        await _opstellingen.AddAsync(opstelling);
        await _opstellingen.SaveChangesAsync();

        var volledig = await _opstellingen.GetByWedstrijdIdAsync(req.WedstrijdId);
        return CreatedAtAction(nameof(Get), new { wedstrijdId = req.WedstrijdId }, Map(volledig!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<OpstellingResponse>> WerkBij(int id, [FromBody] MaakOpstellingRequest req)
    {
        var opstelling = await _opstellingen.GetByIdAsync(id);
        if (opstelling is null)
            throw new NietGevondenException($"Opstelling met id {id} bestaat niet.");

        var wedstrijdId = opstelling.WedstrijdId ?? req.WedstrijdId;
        var wedstrijd = await _wedstrijden.GetByIdAsync(wedstrijdId);
        if (wedstrijd is null)
            throw new NietGevondenException($"Wedstrijd met id {wedstrijdId} bestaat niet.");

        var team = await _teams.GetByIdAsync(wedstrijd.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        var volledig = await _opstellingen.GetByWedstrijdIdAsync(wedstrijdId);
        if (volledig is not null)
        {
            foreach (var oud in volledig.Spelers.ToList())
                volledig.Spelers.Remove(oud);

            volledig.Formatie = req.Formatie;
            foreach (var s in req.Spelers)
            {
                volledig.Spelers.Add(new OpstellingSpeler
                {
                    SpelerId = s.SpelerId,
                    PositieNaam = s.PositieNaam,
                    VeldPositieX = s.VeldPositieX,
                    VeldPositieY = s.VeldPositieY
                });
            }

            _opstellingen.Update(volledig);
            await _opstellingen.SaveChangesAsync();
        }

        var resultaat = await _opstellingen.GetByWedstrijdIdAsync(wedstrijdId);
        return Ok(Map(resultaat!));
    }

    private static OpstellingResponse Map(Opstelling o) => new()
    {
        OpstellingId = o.OpstellingId,
        WedstrijdId = o.WedstrijdId,
        Formatie = o.Formatie,
        Spelers = o.Spelers.Select(s => new OpstellingSpelerDetailDto
        {
            SpelerId = s.SpelerId,
            PositieNaam = s.PositieNaam,
            VeldPositieX = s.VeldPositieX,
            VeldPositieY = s.VeldPositieY,
            SpelerNaam = s.Speler is null ? string.Empty : $"{s.Speler.Voornaam} {s.Speler.Achternaam}".Trim(),
            Rugnummer = s.Speler?.Rugnummer
        }).ToList()
    };
}
