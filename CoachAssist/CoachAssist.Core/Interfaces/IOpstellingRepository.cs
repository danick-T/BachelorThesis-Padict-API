using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IOpstellingRepository : IRepository<Opstelling>
{
    Task<Opstelling?> GetByWedstrijdIdAsync(int wedstrijdId);
}
