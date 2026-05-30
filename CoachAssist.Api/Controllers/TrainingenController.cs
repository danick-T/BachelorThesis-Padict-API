using CoachAssist.Api.Authorization;
using CoachAssist.Api.Extensions;
using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Training;
using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Controllers;

[ApiController]
[Route("api/trainingen")]
[Authorize]
public class TrainingenController : ControllerBase
{
    private readonly ITrainingRepository _trainingen;
    private readonly ITeamRepository _teams;
    private readonly ISpelerRepository _spelers;
    private readonly IPlanningService _planning;
    private readonly AppDbContext _db;

    public TrainingenController(ITrainingRepository trainingen, ITeamRepository teams, ISpelerRepository spelers,
        IPlanningService planning, AppDbContext db)
    {
        _trainingen = trainingen;
        _teams = teams;
        _spelers = spelers;
        _planning = planning;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrainingResponse>>> Lijst([FromQuery] int teamId)
    {
        if (!await MagTeamLezenAsync(teamId))
            return Forbid();

        var lijst = await _trainingen.GetByTeamIdAsync(teamId);
        return Ok(lijst.Select(Map));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrainingResponse>> Get(int id)
    {
        var training = await _trainingen.GetByIdAsync(id);
        if (training is null)
            throw new NietGevondenException($"Training met id {id} bestaat niet.");

        if (!await MagTeamLezenAsync(training.TeamId))
            return Forbid();

        return Ok(Map(training));
    }

    [HttpPost]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TrainingResponse>> Maak([FromBody] MaakTrainingRequest req)
    {
        var team = await _teams.GetByIdAsync(req.TeamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {req.TeamId} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (req.EindTijd <= req.StartTijd)
            throw new DomeinFoutException("EindTijd moet na StartTijd liggen.");

        await _planning.CheckOfThrowAsync(req.TerreinId, req.Datum, req.StartTijd, req.EindTijd);

        var training = new Training
        {
            Datum = req.Datum,
            StartTijd = req.StartTijd,
            EindTijd = req.EindTijd,
            TeamId = req.TeamId,
            TerreinId = req.TerreinId,
            Status = TrainingStatus.Gepland,
            Opmerking = req.Opmerking
        };

        await _trainingen.AddAsync(training);
        await _trainingen.SaveChangesAsync();

        await MaakAanwezigheidsrecordsAsync(training.TrainingId, req.TeamId);

        var volledig = await _trainingen.GetByIdAsync(training.TrainingId);
        return CreatedAtAction(nameof(Get), new { id = training.TrainingId }, Map(volledig!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TrainingResponse>> WerkBij(int id, [FromBody] MaakTrainingRequest req)
    {
        var training = await _trainingen.GetByIdAsync(id);
        if (training is null)
            throw new NietGevondenException($"Training met id {id} bestaat niet.");

        var team = await _teams.GetByIdAsync(training.TeamId);
        if (team is null || team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (req.EindTijd <= req.StartTijd)
            throw new DomeinFoutException("EindTijd moet na StartTijd liggen.");

        var tijdGewijzigd = training.Datum.Date != req.Datum.Date
                            || training.StartTijd != req.StartTijd
                            || training.EindTijd != req.EindTijd
                            || training.TerreinId != req.TerreinId;

        if (tijdGewijzigd)
        {
            await _planning.CheckOfThrowAsync(req.TerreinId, req.Datum, req.StartTijd, req.EindTijd,
                negeerTrainingId: id);
        }

        training.Datum = req.Datum;
        training.StartTijd = req.StartTijd;
        training.EindTijd = req.EindTijd;
        training.TerreinId = req.TerreinId;
        training.Opmerking = req.Opmerking;

        _trainingen.Update(training);
        await _trainingen.SaveChangesAsync();

        var volledig = await _trainingen.GetByIdAsync(id);
        return Ok(Map(volledig!));
    }

    [HttpPost("reeks")]
    [Authorize(Policy = Policies.CoachOnly)]
    public async Task<ActionResult<TrainingReeksResponse>> MaakReeks([FromBody] MaakTrainingReeksRequest req)
    {
        var team = await _teams.GetByIdAsync(req.TeamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {req.TeamId} bestaat niet.");

        if (team.CoachId != User.GetGebruikerId())
            return Forbid();

        if (req.EindTijd <= req.StartTijd)
            throw new DomeinFoutException("EindTijd moet na StartTijd liggen.");

        if (req.LaatsteDatum < req.EersteDatum)
            throw new DomeinFoutException("LaatsteDatum moet na EersteDatum liggen.");

        var response = new TrainingReeksResponse();

        for (var datum = req.EersteDatum.Date; datum <= req.LaatsteDatum.Date; datum = datum.AddDays(1))
        {
            if (datum.DayOfWeek != req.DagVanDeWeek) continue;

            try
            {
                await _planning.CheckOfThrowAsync(req.TerreinId, datum, req.StartTijd, req.EindTijd);
            }
            catch (PlanningConflictException)
            {
                response.Overgeslagen.Add(datum);
                continue;
            }

            var training = new Training
            {
                Datum = datum,
                StartTijd = req.StartTijd,
                EindTijd = req.EindTijd,
                TeamId = req.TeamId,
                TerreinId = req.TerreinId,
                Status = TrainingStatus.Gepland
            };

            await _trainingen.AddAsync(training);
            await _trainingen.SaveChangesAsync();

            await MaakAanwezigheidsrecordsAsync(training.TrainingId, req.TeamId);

            var volledig = await _trainingen.GetByIdAsync(training.TrainingId);
            response.Nieuwe.Add(Map(volledig!));
            response.Aangemaakt++;
        }

        return Ok(response);
    }

    private async Task MaakAanwezigheidsrecordsAsync(int trainingId, int teamId)
    {
        var spelerIds = await _db.Spelers
            .Where(s => s.TeamId == teamId)
            .Select(s => s.SpelerId)
            .ToListAsync();

        foreach (var spelerId in spelerIds)
        {
            _db.TrainingAanwezigheden.Add(new TrainingAanwezigheid
            {
                TrainingId = trainingId,
                SpelerId = spelerId,
                Aanwezig = false,
                Reden = null
            });
        }

        if (spelerIds.Count > 0)
            await _db.SaveChangesAsync();
    }

    private static TrainingResponse Map(Training t) => new()
    {
        TrainingId = t.TrainingId,
        Datum = t.Datum,
        StartTijd = t.StartTijd,
        EindTijd = t.EindTijd,
        TeamId = t.TeamId,
        TerreinId = t.TerreinId,
        Status = t.Status,
        Opmerking = t.Opmerking,
        TerreinNaam = t.Terrein?.Naam ?? string.Empty
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
