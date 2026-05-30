using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IStatistiekRepository : IRepository<SpelerStatistiek>
{
    Task<IEnumerable<SpelerStatistiek>> GetByWedstrijdIdAsync(int wedstrijdId);
    Task<SpelerStatistiek?> ZoekAsync(int spelerId, int wedstrijdId);
    Task<IEnumerable<SpelerStatistiek>> GetByTeamIdAsync(int teamId);
    Task<IEnumerable<SpelerStatistiek>> GetBySpelerIdAsync(int spelerId);
}
