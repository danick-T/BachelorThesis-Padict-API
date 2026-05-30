using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class WedstrijdRepository : BaseRepository<Wedstrijd>, IWedstrijdRepository
{
    public WedstrijdRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Wedstrijd>> GetByTeamIdAsync(int teamId) =>
        await Set.AsNoTracking()
            .Where(w => w.TeamId == teamId)
            .OrderBy(w => w.Datum).ThenBy(w => w.StartTijd)
            .ToListAsync();

    public Task<Wedstrijd?> GetMetDetailsAsync(int id) =>
        Set.AsNoTracking()
            .Include(w => w.Team)
            .Include(w => w.Terrein)
            .Include(w => w.Opstelling)
            .FirstOrDefaultAsync(w => w.WedstrijdId == id);
}
