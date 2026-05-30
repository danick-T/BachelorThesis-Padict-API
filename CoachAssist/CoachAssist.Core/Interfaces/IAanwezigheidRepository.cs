using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IAanwezigheidRepository : IRepository<TrainingAanwezigheid>
{
    Task<IEnumerable<TrainingAanwezigheid>> GetByTrainingIdAsync(int trainingId);
    Task<IEnumerable<TrainingAanwezigheid>> GetBySpelerIdAsync(int spelerId);
    Task<TrainingAanwezigheid?> ZoekAsync(int trainingId, int spelerId);
}
