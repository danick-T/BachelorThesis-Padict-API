using CoachAssist.Api.Middleware;
using CoachAssist.Core.DTOs.Statistiek;
using CoachAssist.Core.Entities;
using CoachAssist.Data;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Services;

public class StatistiekService
{
    private readonly AppDbContext _db;

    public StatistiekService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SeizoenTeamOverzicht> GetSeizoenOverzichtAsync(int teamId)
    {
        var team = await _db.Teams.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TeamId == teamId);
        if (team is null)
            throw new NietGevondenException($"Team met id {teamId} bestaat niet.");

        var stats = await _db.SpelerStatistieken.AsNoTracking()
            .Include(s => s.Speler)
            .Include(s => s.Wedstrijd)
            .Where(s => s.Wedstrijd.TeamId == teamId)
            .ToListAsync();

        var statsPerSpeler = stats
            .GroupBy(s => s.SpelerId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var spelers = await _db.Spelers.AsNoTracking()
            .Where(s => s.TeamId == teamId)
            .OrderBy(s => s.Rugnummer ?? int.MaxValue)
            .ThenBy(s => s.Achternaam)
            .ThenBy(s => s.Voornaam)
            .ToListAsync();

        var spelerStats = spelers
            .Select(speler =>
            {
                statsPerSpeler.TryGetValue(speler.SpelerId, out var spelerWedstrijden);
                spelerWedstrijden ??= new List<SpelerStatistiek>();

                return new SeizoenSpelerStats
                {
                    SpelerId = speler.SpelerId,
                    Voornaam = speler.Voornaam,
                    Achternaam = speler.Achternaam,
                    Rugnummer = speler.Rugnummer,
                    TotaalGoals = spelerWedstrijden.Sum(s => s.Goals ?? 0),
                    TotaalAssists = spelerWedstrijden.Sum(s => s.Assists ?? 0),
                    TotaalGeleKaarten = spelerWedstrijden.Sum(s => s.GeleKaarten ?? 0),
                    TotaalRodeKaarten = spelerWedstrijden.Sum(s => s.RodeKaarten ?? 0),
                    TotaalMinuten = spelerWedstrijden.Sum(s => s.GespeeldeMinuten ?? 0),
                    AantalWedstrijden = spelerWedstrijden.Count
                };
            })
            .OrderByDescending(s => s.TotaalGoals)
            .ThenBy(s => s.Rugnummer ?? int.MaxValue)
            .ToList();

        return new SeizoenTeamOverzicht
        {
            TeamId = team.TeamId,
            TeamNaam = team.Naam,
            Seizoen = team.Seizoen,
            Spelers = spelerStats
        };
    }

    public async Task<SeizoenSpelerStats> GetSpelerSeizoenAsync(int spelerId)
    {
        var speler = await _db.Spelers.AsNoTracking()
            .FirstOrDefaultAsync(s => s.SpelerId == spelerId);
        if (speler is null)
            throw new NietGevondenException($"Speler met id {spelerId} bestaat niet.");

        var stats = await _db.SpelerStatistieken.AsNoTracking()
            .Include(s => s.Wedstrijd)
            .Where(s => s.SpelerId == spelerId)
            .Where(s => s.Wedstrijd.TeamId == speler.TeamId)
            .ToListAsync();

        return new SeizoenSpelerStats
        {
            SpelerId = speler.SpelerId,
            Voornaam = speler.Voornaam,
            Achternaam = speler.Achternaam,
            Rugnummer = speler.Rugnummer,
            TotaalGoals = stats.Sum(s => s.Goals ?? 0),
            TotaalAssists = stats.Sum(s => s.Assists ?? 0),
            TotaalGeleKaarten = stats.Sum(s => s.GeleKaarten ?? 0),
            TotaalRodeKaarten = stats.Sum(s => s.RodeKaarten ?? 0),
            TotaalMinuten = stats.Sum(s => s.GespeeldeMinuten ?? 0),
            AantalWedstrijden = stats.Count
        };
    }
}
