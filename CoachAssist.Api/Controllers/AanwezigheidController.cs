using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Aanwezigheid;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/aanwezigheid")]
[Authorize]
public class AanwezigheidController : ControllerBase
{
    private readonly IAanwezigheidRepository _aanwezigheden;
    private readonly ITrainingRepository _trainingen;
    private readonly ITeamRepository _teams;
    private readonly AppDbContext _db;

    public AanwezigheidController(IAanwezigheidRepository aanwezigheden, ITrainingRepository trainingen,
        ITeamRepository teams, AppDbContext db)
    {
        _aanwezigheden = aanwezigheden;
        _trainingen = trainingen;
        _teams = teams;
        _db = db;
    }

    [HttpPost]
    [Authorize(Policy = Policies.SpelerOnly)]
    public async Task<ActionResult<AanwezigheidResponse>> Bevestig([FromBody] BevestigAanwezigheidRequest req)
    {
        var gebruikerId = User.GetGebruikerId();
        var speler = await _db.Spelers.FirstOrDefaultAsync(s => s.GebruikerId == gebruikerId);
        if (speler is null)
            throw new DomeinFoutException("Ingelogde gebruiker is niet gekoppeld aan een speler.");

        var training = await _trainingen.GetByIdAsync(req.TrainingId);
        if (training is null)
            throw new NietGevondenException($"Training met id {req.TrainingId} bestaat niet.");

        if (training.TeamId != speler.TeamId)
            return Forbid();

        var bestaand = await _aanwezigheden.ZoekAsync(req.TrainingId, speler.SpelerId);

        if (bestaand is null)
        {
            bestaand = new TrainingAanwezigheid
            {
                TrainingId = req.TrainingId,
                SpelerId = speler.SpelerId,
                Aanwezig = req.Aanwezig,
                Reden = req.Reden
            };
            await _aanwezigheden.AddAsync(bestaand);
        }
        else
        {
            bestaand.Aanwezig = req.Aanwezig;
            bestaand.Reden = req.Reden;
            _aanwezigheden.Update(bestaand);
        }

        await _aanwezigheden.SaveChangesAsync();

        var volledig = await _aanwezigheden.GetByIdAsync(bestaand.AanwezigheidId);
        return Ok(Map(volledig!));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AanwezigheidResponse>> WerkBij(int id, [FromBody] WerkAanwezigheidBijRequest req)
    {
        var aanwezigheid = await _aanwezigheden.GetByIdAsync(id);
        if (aanwezigheid is null)
            throw new NietGevondenException($"Aanwezigheid met id {id} bestaat niet.");

        var gebruikerId = User.GetGebruikerId();
        var isSpelerEigenaar = aanwezigheid.Speler.GebruikerId == gebruikerId;
        var isCoachVanTeam = aanwezigheid.Training.Team.CoachId == gebruikerId;

        if (!isSpelerEigenaar && !isCoachVanTeam)
            return Forbid();

        aanwezigheid.Aanwezig = req.Aanwezig;
        aanwezigheid.Reden = req.Reden;

        _aanwezigheden.Update(aanwezigheid);
        await _aanwezigheden.SaveChangesAsync();

        return Ok(Map(aanwezigheid));
    }

    [HttpGet("training/{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<IEnumerable<AanwezigheidResponse>>> GetByTraining(int id)
    {
        var training = await _trainingen.GetByIdAsync(id);
        if (training is null)
            throw new NietGevondenException($"Training met id {id} bestaat niet.");

        var team = await _teams.GetByIdAsync(training.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        var lijst = await _aanwezigheden.GetByTrainingIdAsync(id);
        return Ok(lijst.Select(Map));
    }

    [HttpGet("speler/{id:int}")]
    public async Task<ActionResult<IEnumerable<AanwezigheidResponse>>> GetBySpeler(int id)
    {
        var speler = await _db.Spelers.FirstOrDefaultAsync(s => s.SpelerId == id);
        if (speler is null)
            throw new NietGevondenException($"Speler met id {id} bestaat niet.");

        var gebruikerId = User.GetGebruikerId();
        var team = await _teams.GetByIdAsync(speler.TeamId);
        var isSpelerZelf = speler.GebruikerId == gebruikerId;
        var isCoach = team is not null && team.CoachId == gebruikerId;

        if (!isSpelerZelf && !isCoach)
            return Forbid();

        var lijst = await _aanwezigheden.GetBySpelerIdAsync(id);
        return Ok(lijst.Select(Map));
    }

    private static AanwezigheidResponse Map(TrainingAanwezigheid a) => new()
    {
        AanwezigheidId = a.AanwezigheidId,
        TrainingId = a.TrainingId,
        SpelerId = a.SpelerId,
        SpelerNaam = a.Speler is null ? string.Empty : $"{a.Speler.Voornaam} {a.Speler.Achternaam}",
        Aanwezig = a.Aanwezig,
        Reden = a.Reden
    };
}
