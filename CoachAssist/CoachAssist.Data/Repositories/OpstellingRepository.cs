using CoachAssist.Core.Entities;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class OpstellingRepository : BaseRepository<Opstelling>, IOpstellingRepository
{
    public OpstellingRepository(AppDbContext db) : base(db) { }

    public Task<Opstelling?> GetByWedstrijdIdAsync(int wedstrijdId) =>
        Set
            .Include(o => o.Spelers)
                .ThenInclude(os => os.Speler)
            .FirstOrDefaultAsync(o => o.WedstrijdId == wedstrijdId);
}
