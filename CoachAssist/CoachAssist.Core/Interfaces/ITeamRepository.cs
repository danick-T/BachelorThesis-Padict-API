using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team?> GetMetSpelersAsync(int teamId);
    Task<IEnumerable<Team>> GetByClubIdAsync(int clubId);
    Task<IEnumerable<Team>> GetByCoachIdAsync(int coachId);
}
