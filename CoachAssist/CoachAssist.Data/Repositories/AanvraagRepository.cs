using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public class AanvraagRepository : IAanvraagRepository
{
    private readonly AppDbContext _db;

    public AanvraagRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<ClubAanvraag?> GetClubAanvraagAsync(int id) =>
        _db.ClubAanvragen
            .Include(a => a.Gebruiker)
            .Include(a => a.Club)
            .FirstOrDefaultAsync(a => a.AanvraagId == id);

    public Task<TeamAanvraag?> GetTeamAanvraagAsync(int id) =>
        _db.TeamAanvragen
            .Include(a => a.Speler)
            .Include(a => a.Team)
            .FirstOrDefaultAsync(a => a.AanvraagId == id);

    public async Task<IEnumerable<ClubAanvraag>> GetOpenstaandeClubAanvragenAsync(int clubId)
    {
        return await _db.ClubAanvragen
            .AsNoTracking()
            .Include(a => a.Gebruiker)
            .Include(a => a.Club)
            .Where(a => a.ClubId == clubId && a.Status == AanvraagStatus.InAfwachting)
            .OrderBy(a => a.DatumAanvraag)
            .ToListAsync();
    }

    public async Task<IEnumerable<TeamAanvraag>> GetOpenstaandeTeamAanvragenAsync(int teamId)
    {
        return await _db.TeamAanvragen
            .AsNoTracking()
            .Include(a => a.Speler)
            .Include(a => a.Team)
            .Where(a => a.TeamId == teamId && a.Status == AanvraagStatus.InAfwachting)
            .OrderBy(a => a.DatumAanvraag)
            .ToListAsync();
    }

    public Task<bool> HeeftGoedgekeurdeClubAanvraagAsync(int gebruikerId, int clubId) =>
        _db.ClubAanvragen
            .AsNoTracking()
            .AnyAsync(a => a.GebruikerId == gebruikerId
                        && a.ClubId == clubId
                        && a.Status == AanvraagStatus.Goedgekeurd);

    public Task<ClubAanvraag?> ZoekClubAanvraagAsync(int gebruikerId, int clubId) =>
        _db.ClubAanvragen
            .FirstOrDefaultAsync(a => a.GebruikerId == gebruikerId && a.ClubId == clubId);

    public Task<TeamAanvraag?> ZoekTeamAanvraagAsync(int spelerId, int teamId) =>
        _db.TeamAanvragen
            .FirstOrDefaultAsync(a => a.SpelerId == spelerId && a.TeamId == teamId);

    public async Task AddClubAanvraagAsync(ClubAanvraag aanvraag) =>
        await _db.ClubAanvragen.AddAsync(aanvraag);

    public async Task AddTeamAanvraagAsync(TeamAanvraag aanvraag) =>
        await _db.TeamAanvragen.AddAsync(aanvraag);

    public void UpdateClub(ClubAanvraag aanvraag) =>
        _db.Entry(aanvraag).Property(a => a.Status).IsModified = true;

    public void UpdateTeam(TeamAanvraag aanvraag)
    {
        _db.Entry(aanvraag).Property(a => a.Status).IsModified = true;
        if (aanvraag.Speler is not null)
        {
            _db.Entry(aanvraag.Speler).Property(s => s.TeamId).IsModified = true;
        }
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
