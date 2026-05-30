using CoachAssist.Core.Entities;

namespace CoachAssist.Core.Interfaces;

public interface IAanvraagRepository
{
    Task<ClubAanvraag?> GetClubAanvraagAsync(int id);
    Task<TeamAanvraag?> GetTeamAanvraagAsync(int id);
    Task<IEnumerable<ClubAanvraag>> GetOpenstaandeClubAanvragenAsync(int clubId);
    Task<IEnumerable<TeamAanvraag>> GetOpenstaandeTeamAanvragenAsync(int teamId);
    Task<bool> HeeftGoedgekeurdeClubAanvraagAsync(int gebruikerId, int clubId);
    Task<ClubAanvraag?> ZoekClubAanvraagAsync(int gebruikerId, int clubId);
    Task<TeamAanvraag?> ZoekTeamAanvraagAsync(int spelerId, int teamId);
    Task AddClubAanvraagAsync(ClubAanvraag aanvraag);
    Task AddTeamAanvraagAsync(TeamAanvraag aanvraag);
    void UpdateClub(ClubAanvraag aanvraag);
    void UpdateTeam(TeamAanvraag aanvraag);
    Task SaveChangesAsync();
}
