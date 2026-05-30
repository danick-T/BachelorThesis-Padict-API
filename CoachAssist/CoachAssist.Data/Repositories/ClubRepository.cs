using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class ClubRepository : BaseRepository<Club>, IClubRepository
{
    public ClubRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Club>> ZoekAsync(string? term)
    {
        var query = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var patroon = $"%{term.Trim()}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Naam, patroon) ||
                EF.Functions.ILike(c.Stad, patroon));
        }

        return await query.OrderBy(c => c.Naam).ToListAsync();
    }

    public Task<bool> BestaatNaamAsync(string naam)
    {
        return Set.AsNoTracking().AnyAsync(c => c.Naam.ToLower() == naam.ToLower());
    }
}
