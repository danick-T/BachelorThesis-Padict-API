namespace CoachAssist.Core.Interfaces;

public interface IPlanningService
{
    Task CheckOfThrowAsync(int terreinId, DateTime datum, TimeSpan startTijd, TimeSpan eindTijd,
        int? negeerWedstrijdId = null, int? negeerTrainingId = null);
}
