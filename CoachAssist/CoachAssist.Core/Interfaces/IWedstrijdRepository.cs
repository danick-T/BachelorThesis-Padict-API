using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IWedstrijdRepository : IRepository<Wedstrijd>
{
    Task<IEnumerable<Wedstrijd>> GetByTeamIdAsync(int teamId);
    Task<Wedstrijd?> GetMetDetailsAsync(int id);
}
