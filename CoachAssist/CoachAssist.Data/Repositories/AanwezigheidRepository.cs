using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class AanwezigheidRepository : BaseRepository<TrainingAanwezigheid>, IAanwezigheidRepository
{
    public AanwezigheidRepository(AppDbContext db) : base(db) { }

    public override Task<TrainingAanwezigheid?> GetByIdAsync(int id) =>
        Set.Include(a => a.Speler)
            .Include(a => a.Training).ThenInclude(t => t.Team)
            .FirstOrDefaultAsync(a => a.AanwezigheidId == id);

    public async Task<IEnumerable<TrainingAanwezigheid>> GetByTrainingIdAsync(int trainingId) =>
        await Set.Include(a => a.Speler)
            .Where(a => a.TrainingId == trainingId)
            .ToListAsync();

    public async Task<IEnumerable<TrainingAanwezigheid>> GetBySpelerIdAsync(int spelerId) =>
        await Set.Include(a => a.Speler)
            .Include(a => a.Training)
            .Where(a => a.SpelerId == spelerId)
            .OrderByDescending(a => a.Training.Datum)
            .ToListAsync();

    public Task<TrainingAanwezigheid?> ZoekAsync(int trainingId, int spelerId) =>
        Set.FirstOrDefaultAsync(a => a.TrainingId == trainingId && a.SpelerId == spelerId);
}
