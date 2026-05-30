using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class SpelerRepository : BaseRepository<Speler>, ISpelerRepository
{
    public SpelerRepository(AppDbContext db) : base(db) { }

    public override Task<Speler?> GetByIdAsync(int id) =>
        Set.Include(s => s.Team)
            .Include(s => s.Gebruiker)
            .FirstOrDefaultAsync(s => s.SpelerId == id);

    public async Task<IEnumerable<Speler>> GetByTeamIdAsync(int teamId) =>
        await Set.AsNoTracking()
            .Where(s => s.TeamId == teamId)
            .ToListAsync();

    public Task<Speler?> GetByGebruikerIdAsync(int gebruikerId) =>
        Set.AsNoTracking()
            .Include(s => s.Team)
            .Include(s => s.Gebruiker)
            .FirstOrDefaultAsync(s => s.GebruikerId == gebruikerId);
}
