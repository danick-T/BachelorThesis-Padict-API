using CoachAssist.Api.Middleware;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using CoachAssist.Data;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Api.Services;

public class PlanningService : IPlanningService
{
    private static readonly TimeSpan WedstrijdDuur = TimeSpan.FromMinutes(120);

    private readonly AppDbContext _db;

    public PlanningService(AppDbContext db)
    {
        _db = db;
    }

    public async Task CheckOfThrowAsync(int terreinId, DateTime datum, TimeSpan startTijd, TimeSpan eindTijd,
        int? negeerWedstrijdId = null, int? negeerTrainingId = null)
    {
        var dag = datum.Date;

        var wedstrijden = await _db.Wedstrijden
            .Where(w => w.TerreinId == terreinId
                        && w.Datum.Date == dag
                        && w.Status != WedstrijdStatus.Geannuleerd
                        && (negeerWedstrijdId == null || w.WedstrijdId != negeerWedstrijdId))
            .ToListAsync();

        foreach (var w in wedstrijden)
        {
            var wEind = w.StartTijd + WedstrijdDuur;
            if (startTijd < wEind && eindTijd > w.StartTijd)
            {
                throw new PlanningConflictException(
                    $"Conflict met wedstrijd op {dag:yyyy-MM-dd} {w.StartTijd}.");
            }
        }

        var trainingen = await _db.Trainingen
            .Where(t => t.TerreinId == terreinId
                        && t.Datum.Date == dag
                        && t.Status != TrainingStatus.Afgelast
                        && (negeerTrainingId == null || t.TrainingId != negeerTrainingId))
            .ToListAsync();

        foreach (var t in trainingen)
        {
            if (startTijd < t.EindTijd && eindTijd > t.StartTijd)
            {
                throw new PlanningConflictException(
                    $"Conflict met training op {dag:yyyy-MM-dd} {t.StartTijd}-{t.EindTijd}.");
            }
        }
    }
}
