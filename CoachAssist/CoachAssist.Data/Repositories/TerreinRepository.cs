using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class TerreinRepository : BaseRepository<Terrein>, ITerreinRepository
{
    public TerreinRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Terrein>> GetByClubIdAsync(int clubId)
    {
        return await Set.AsNoTracking()
            .Where(t => t.ClubId == clubId)
            .OrderBy(t => t.Naam)
            .ToListAsync();
    }

    public Task<bool> BestaatNaamInClubAsync(string naam, int clubId)
    {
        return Set.AsNoTracking()
            .AnyAsync(t => t.ClubId == clubId && t.Naam.ToLower() == naam.ToLower());
    }
}
