using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class TrainingRepository : BaseRepository<Training>, ITrainingRepository
{
    public TrainingRepository(AppDbContext db) : base(db) { }

    public override Task<Training?> GetByIdAsync(int id) =>
        Set.Include(t => t.Terrein).FirstOrDefaultAsync(t => t.TrainingId == id);

    public async Task<IEnumerable<Training>> GetByTeamIdAsync(int teamId) =>
        await Set.Include(t => t.Terrein)
            .Where(t => t.TeamId == teamId)
            .OrderBy(t => t.Datum).ThenBy(t => t.StartTijd)
            .ToListAsync();

    public Task<Training?> GetMetAanwezighedenAsync(int id) =>
        Set.Include(t => t.Terrein)
            .Include(t => t.Aanwezigheden).ThenInclude(a => a.Speler)
            .FirstOrDefaultAsync(t => t.TrainingId == id);

    public async Task<IEnumerable<Training>> GetByTerreinEnDatumAsync(int terreinId, DateTime datum)
    {
        var dag = datum.Date;
        return await Set.Where(t => t.TerreinId == terreinId && t.Datum.Date == dag).ToListAsync();
    }
}
