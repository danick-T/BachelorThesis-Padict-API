using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface ISpelerRepository : IRepository<Speler>
{
    Task<IEnumerable<Speler>> GetByTeamIdAsync(int teamId);
    Task<Speler?> GetByGebruikerIdAsync(int gebruikerId);
}
