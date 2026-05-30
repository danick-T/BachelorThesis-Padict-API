using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IClubRepository : IRepository<Club>
{
    Task<IEnumerable<Club>> ZoekAsync(string? term);
    Task<bool> BestaatNaamAsync(string naam);
}
