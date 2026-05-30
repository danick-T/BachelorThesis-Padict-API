using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface ITerreinRepository : IRepository<Terrein>
{
    Task<IEnumerable<Terrein>> GetByClubIdAsync(int clubId);
    Task<bool> BestaatNaamInClubAsync(string naam, int clubId);
}
