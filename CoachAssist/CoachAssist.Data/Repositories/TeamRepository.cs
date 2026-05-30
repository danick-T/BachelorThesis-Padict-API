using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class TeamRepository : BaseRepository<Team>, ITeamRepository
{
    public TeamRepository(AppDbContext db) : base(db) { }

    public Task<Team?> GetMetSpelersAsync(int teamId) =>
        Set.AsNoTracking()
            .Include(t => t.Spelers)
            .Include(t => t.Coach)
            .Include(t => t.Club)
            .FirstOrDefaultAsync(t => t.TeamId == teamId);

    public async Task<IEnumerable<Team>> GetByClubIdAsync(int clubId) =>
        await Set.AsNoTracking()
            .Include(t => t.Coach)
            .Where(t => t.ClubId == clubId)
            .ToListAsync();

    public async Task<IEnumerable<Team>> GetByCoachIdAsync(int coachId) =>
        await Set.AsNoTracking()
            .Include(t => t.Coach)
            .Where(t => t.CoachId == coachId)
            .ToListAsync();
}
