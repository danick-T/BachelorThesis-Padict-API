using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface ITrainingRepository : IRepository<Training>
{
    Task<IEnumerable<Training>> GetByTeamIdAsync(int teamId);
    Task<Training?> GetMetAanwezighedenAsync(int id);
    Task<IEnumerable<Training>> GetByTerreinEnDatumAsync(int terreinId, DateTime datum);
}
