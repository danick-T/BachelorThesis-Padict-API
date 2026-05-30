using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class StatistiekRepository : BaseRepository<SpelerStatistiek>, IStatistiekRepository
{
    public StatistiekRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<SpelerStatistiek>> GetByWedstrijdIdAsync(int wedstrijdId) =>
        await Set.AsNoTracking()
            .Include(s => s.Speler)
            .Where(s => s.WedstrijdId == wedstrijdId)
            .ToListAsync();

    public Task<SpelerStatistiek?> ZoekAsync(int spelerId, int wedstrijdId) =>
        Set.FirstOrDefaultAsync(s => s.SpelerId == spelerId && s.WedstrijdId == wedstrijdId);

    public async Task<IEnumerable<SpelerStatistiek>> GetByTeamIdAsync(int teamId) =>
        await Set.AsNoTracking()
            .Include(s => s.Speler)
            .Include(s => s.Wedstrijd)
            .Where(s => s.Wedstrijd.TeamId == teamId)
            .ToListAsync();

    public async Task<IEnumerable<SpelerStatistiek>> GetBySpelerIdAsync(int spelerId) =>
        await Set.AsNoTracking()
            .Include(s => s.Speler)
            .Include(s => s.Wedstrijd)
            .Where(s => s.SpelerId == spelerId)
            .ToListAsync();
}
