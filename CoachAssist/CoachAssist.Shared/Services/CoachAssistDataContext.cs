using CoachAssist.Core.DTOs.Aanvraag;
using CoachAssist.Core.DTOs.Aanwezigheid;
using CoachAssist.Core.DTOs.Club;
using CoachAssist.Core.DTOs.Opstelling;
using CoachAssist.Core.DTOs.Speler;
using CoachAssist.Core.DTOs.Statistiek;
using CoachAssist.Core.DTOs.Team;
using CoachAssist.Core.DTOs.Terrein;
using CoachAssist.Core.DTOs.Training;
using CoachAssist.Core.DTOs.Wedstrijd;
using CoachAssist.Shared.Models.SpelerPages;
using System.Net.Http.Json;

namespace CoachAssist.Shared.Services;

public sealed class CoachAssistDataContext
{
    private readonly ICoachAssistApiClient _api;

    public CoachAssistDataContext(ICoachAssistApiClient api)
    {
        _api = api;
    }

    public async Task<ClubResponse?> GetEersteClubAsync(CancellationToken cancellationToken = default)
    {
        var clubs = await _api.GetFromJsonAuthorizedAsync<List<ClubResponse>>("api/clubs", cancellationToken);
        return clubs?.OrderBy(c => c.Naam).FirstOrDefault();
    }

    public async Task<List<ClubResponse>> ZoekClubsAsync(
        string? zoek,
        CancellationToken cancellationToken = default)
    {
        var uri = string.IsNullOrWhiteSpace(zoek)
            ? "api/clubs"
            : $"api/clubs?zoek={Uri.EscapeDataString(zoek)}";

        return await _api.GetFromJsonAuthorizedAsync<List<ClubResponse>>(uri, cancellationToken) ?? new();
    }

    public async Task<List<TeamDetailResponse>> GetTeamsVoorClubAsync(
        int clubId,
        CancellationToken cancellationToken = default)
    {
        var teams = await _api.GetFromJsonAuthorizedAsync<List<ClubTeamListItem>>(
            $"api/clubs/{clubId}/teams",
            cancellationToken) ?? new();

        var details = new List<TeamDetailResponse>();
        foreach (var team in teams)
        {
            var detail = await _api.GetFromJsonAuthorizedAsync<TeamDetailResponse>(
                $"api/teams/{team.TeamId}",
                cancellationToken);

            if (detail is not null)
            {
                details.Add(detail);
            }
        }

        return details.OrderBy(t => t.Naam).ToList();
    }

    public async Task<List<TeamResponse>> GetTeamLijstVoorClubAsync(
        int clubId,
        CancellationToken cancellationToken = default)
    {
        var teams = await _api.GetFromJsonAuthorizedAsync<List<ClubTeamListItem>>(
            $"api/clubs/{clubId}/teams",
            cancellationToken) ?? new();

        return teams
            .OrderBy(t => t.Naam)
            .Select(t => new TeamResponse
            {
                TeamId = t.TeamId,
                Naam = t.Naam,
                Seizoen = t.Seizoen,
                ClubId = clubId
            })
            .ToList();
    }

    public async Task MaakClubAanvraagAsync(
        int clubId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _api.PostAsJsonAuthorizedAsync(
            "api/aanvragen/club",
            new MaakClubAanvraagRequest { ClubId = clubId },
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task MaakTeamAanvraagAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _api.PostAsJsonAuthorizedAsync(
            "api/aanvragen/team",
            new MaakTeamAanvraagRequest { TeamId = teamId },
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<TeamDetailResponse?> GetTeamVoorCoachAsync(CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<TeamDetailResponse>(
            "api/teams/me",
            cancellationToken);
    }

    public async Task<List<SpelerResponse>> GetSpelersVoorTeamAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<SpelerResponse>>(
            $"api/teams/{teamId}/spelers",
            cancellationToken) ?? new();
    }

    public async Task<List<TerreinResponse>> GetTerreinenVoorClubAsync(
        int clubId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<TerreinResponse>>(
            $"api/terreinen?clubId={clubId}",
            cancellationToken) ?? new();
    }

    public async Task<List<TeamAanvraagResponse>> GetOpenTeamAanvragenAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<TeamAanvraagResponse>>(
            $"api/aanvragen/team?teamId={teamId}",
            cancellationToken) ?? new();
    }

    public async Task<List<ClubAanvraagResponse>> GetOpenClubAanvragenAsync(
        int clubId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<ClubAanvraagResponse>>(
            $"api/aanvragen/club?clubId={clubId}",
            cancellationToken) ?? new();
    }

    public async Task<List<TrainingResponse>> GetTrainingenVoorTeamAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<TrainingResponse>>(
            $"api/trainingen?teamId={teamId}",
            cancellationToken) ?? new();
    }

    public async Task<List<AanwezigheidResponse>> GetAanwezighedenVoorTrainingAsync(
        int trainingId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<AanwezigheidResponse>>(
            $"api/aanwezigheid/training/{trainingId}",
            cancellationToken) ?? new();
    }

    public async Task<List<WedstrijdResponse>> GetWedstrijdenVoorTeamAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<WedstrijdResponse>>(
            $"api/wedstrijden?teamId={teamId}",
            cancellationToken) ?? new();
    }

    public async Task<WedstrijdDetailResponse?> GetWedstrijdDetailAsync(
        int wedstrijdId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<WedstrijdDetailResponse>(
            $"api/wedstrijden/{wedstrijdId}",
            cancellationToken);
    }

    public async Task<List<StatistiekResponse>> GetStatistiekenVoorWedstrijdAsync(
        int wedstrijdId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<StatistiekResponse>>(
            $"api/statistieken/wedstrijd/{wedstrijdId}",
            cancellationToken) ?? new();
    }

    public async Task<List<StatistiekResponse>> BewaarWedstrijdStatistiekenAsync(
        int wedstrijdId,
        BulkStatistiekenRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _api.PostAsJsonAuthorizedAsync(
            $"api/statistieken/wedstrijd/{wedstrijdId}",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StatistiekResponse>>(cancellationToken) ?? new();
    }

    public async Task<OpstellingResponse?> GetOpstellingVoorWedstrijdAsync(
        int wedstrijdId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _api.GetFromJsonAuthorizedAsync<OpstellingResponse>(
                $"api/opstellingen/{wedstrijdId}",
                cancellationToken);
        }
        catch (AuthApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<OpstellingResponse?> BewaarOpstellingAsync(
        int? opstellingId,
        MaakOpstellingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (opstellingId is null)
        {
            using var createResponse = await _api.PostAsJsonAuthorizedAsync(
                "api/opstellingen",
                request,
                cancellationToken);

            createResponse.EnsureSuccessStatusCode();
            return await createResponse.Content.ReadFromJsonAsync<OpstellingResponse>(cancellationToken);
        }

        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"api/opstellingen/{opstellingId.Value}")
        {
            Content = JsonContent.Create(request)
        };

        using var updateResponse = await _api.SendAuthorizedAsync(updateRequest, cancellationToken);
        updateResponse.EnsureSuccessStatusCode();
        return await updateResponse.Content.ReadFromJsonAsync<OpstellingResponse>(cancellationToken);
    }

    public async Task<SpelerResponse?> MaakSpelerAsync(
        MaakSpelerRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _api.PostAsJsonAuthorizedAsync(
            "api/spelers",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SpelerResponse>(cancellationToken);
    }

    public async Task<SeizoenTeamOverzicht?> GetSeizoenVoorTeamAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        return await _api.GetFromJsonAuthorizedAsync<SeizoenTeamOverzicht>(
            $"api/teams/{teamId}/seizoen",
            cancellationToken);
    }

    public async Task<SpelerIdentity?> GetHuidigeSpelerAsync(CancellationToken cancellationToken = default)
    {
        var gebruiker = _api.AuthSession.Gebruiker ?? await _api.GetCurrentUserAsync(cancellationToken);
        if (gebruiker is null)
        {
            return null;
        }

        var detail = await _api.GetFromJsonAuthorizedAsync<SpelerDetailResponse>(
            "api/spelers/me",
            cancellationToken);

        if (detail is null)
        {
            return null;
        }

        return new SpelerIdentity(
            detail.SpelerId,
            detail.TeamId,
            detail.Voornaam,
            detail.Achternaam,
            detail.TeamNaam,
            detail.ClubNaam,
            detail.VastePositie?.ToString(),
            detail.Rugnummer,
            detail.Email ?? gebruiker.Email);
    }

    public async Task<SpelerDashboardData?> GetSpelerDashboardAsync(CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return null;
        }

        var seizoen = await GetSpelerSeizoenAsync(speler.SpelerId, cancellationToken);
        var trainingen = await GetTrainingenVoorTeamAsync(speler.TeamId, cancellationToken);
        var wedstrijden = await GetSpelerWedstrijdenAsync(speler, cancellationToken);
        var aanwezigheden = await GetAanwezighedenVoorSpelerAsync(speler.SpelerId, cancellationToken);
        var stats = await GetStatsVoorSpelerAsync(speler.SpelerId, cancellationToken);

        var vandaag = DateTime.Today;
        var volgendeTraining = trainingen
            .Where(t => t.Datum.Date >= vandaag)
            .OrderBy(t => t.Datum)
            .ThenBy(t => t.StartTijd)
            .Select(t => MapTraining(t, aanwezigheden.FirstOrDefault(a => a.TrainingId == t.TrainingId)))
            .FirstOrDefault();

        var wedstrijdCards = wedstrijden
            .Select(w => MapWedstrijd(w, stats.Where(s => s.WedstrijdId == w.WedstrijdId), speler.TeamNaam))
            .ToList();

        return new SpelerDashboardData(
            speler,
            MapSeasonStats(seizoen, stats),
            volgendeTraining,
            wedstrijdCards
                .Where(w => w.Datum.Date >= vandaag)
                .OrderBy(w => w.Datum)
                .ThenBy(w => w.StartTijd)
                .FirstOrDefault(),
            wedstrijdCards
                .Where(w => w.Datum.Date < vandaag || w.ThuisScore is not null || w.UitScore is not null)
                .OrderByDescending(w => w.Datum)
                .Take(3)
                .ToList(),
            aanwezigheden.Count(a => a.Aanwezig),
            aanwezigheden.Count);
    }

    public async Task<IReadOnlyList<SpelerSchemaItem>> GetSpelerSchemaAsync(CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return Array.Empty<SpelerSchemaItem>();
        }

        var trainingen = await GetTrainingenVoorTeamAsync(speler.TeamId, cancellationToken);
        var wedstrijden = await GetSpelerWedstrijdenAsync(speler, cancellationToken);
        var aanwezigheden = await GetAanwezighedenVoorSpelerAsync(speler.SpelerId, cancellationToken);

        var trainingItems = trainingen.Select(t =>
        {
            var aanwezigheid = aanwezigheden.FirstOrDefault(a => a.TrainingId == t.TrainingId);
            return new SpelerSchemaItem(
                t.TrainingId,
                SpelerSchemaType.Training,
                t.Datum,
                t.StartTijd,
                "Training",
                $"{t.TerreinNaam} · {Math.Max(0, (int)(t.EindTijd - t.StartTijd).TotalMinutes)} min",
                aanwezigheid is null ? "Bevestig" : aanwezigheid.Aanwezig ? "Bevestigd" : "Afwezig");
        });

        var wedstrijdItems = wedstrijden.Select(w => new SpelerSchemaItem(
            w.WedstrijdId,
            SpelerSchemaType.Wedstrijd,
            w.Datum,
            w.StartTijd,
            $"{speler.TeamNaam} vs {w.Tegenstander}",
            string.IsNullOrWhiteSpace(w.TerreinNaam) ? "Wedstrijd" : w.TerreinNaam,
            w.Status.ToString()));

        return trainingItems
            .Concat(wedstrijdItems)
            .OrderBy(i => i.Datum)
            .ThenBy(i => i.StartTijd)
            .ToList();
    }

    public async Task<SpelerTrainingAanwezigheidData?> GetSpelerAanwezigheidAsync(
        int? trainingId,
        CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return null;
        }

        var trainingen = await GetTrainingenVoorTeamAsync(speler.TeamId, cancellationToken);
        var aanwezigheden = await GetAanwezighedenVoorSpelerAsync(speler.SpelerId, cancellationToken);
        var training = trainingId.HasValue
            ? trainingen.FirstOrDefault(t => t.TrainingId == trainingId.Value)
            : trainingen.Where(t => t.Datum.Date >= DateTime.Today)
                .OrderBy(t => t.Datum)
                .ThenBy(t => t.StartTijd)
                .FirstOrDefault();

        if (training is null)
        {
            return null;
        }

        List<AanwezigheidResponse> teamReacties;
        try
        {
            teamReacties = await _api.GetFromJsonAuthorizedAsync<List<AanwezigheidResponse>>(
                $"api/aanwezigheid/training/{training.TrainingId}",
                cancellationToken) ?? new();
        }
        catch (AuthApiException)
        {
            teamReacties = new();
        }

        return new SpelerTrainingAanwezigheidData(
            MapTraining(training, aanwezigheden.FirstOrDefault(a => a.TrainingId == training.TrainingId)),
            teamReacties.Count(a => a.Aanwezig),
            teamReacties.Count(a => !a.Aanwezig && !string.IsNullOrWhiteSpace(a.Reden)),
            teamReacties.Count(a => !a.Aanwezig && string.IsNullOrWhiteSpace(a.Reden)));
    }

    public async Task BevestigSpelerAanwezigheidAsync(
        int trainingId,
        bool aanwezig,
        string? reden,
        CancellationToken cancellationToken = default)
    {
        using var response = await _api.PostAsJsonAuthorizedAsync(
            "api/aanwezigheid",
            new BevestigAanwezigheidRequest
            {
                TrainingId = trainingId,
                Aanwezig = aanwezig,
                Reden = reden
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<SpelerWedstrijdDetailData?> GetSpelerWedstrijdDetailAsync(
        int? wedstrijdId,
        CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return null;
        }

        var wedstrijden = await GetSpelerWedstrijdenAsync(speler, cancellationToken);
        var wedstrijd = wedstrijdId.HasValue
            ? wedstrijden.FirstOrDefault(w => w.WedstrijdId == wedstrijdId.Value)
            : wedstrijden.OrderByDescending(w => w.Datum).FirstOrDefault();

        if (wedstrijd is null)
        {
            return null;
        }

        var seizoen = await GetSpelerSeizoenAsync(speler.SpelerId, cancellationToken);
        var wedstrijdStats = await _api.GetFromJsonAuthorizedAsync<List<StatistiekResponse>>(
            $"api/statistieken/wedstrijd/{wedstrijd.WedstrijdId}",
            cancellationToken) ?? new();

        return new SpelerWedstrijdDetailData(
            speler,
            MapWedstrijd(wedstrijd, wedstrijdStats.Where(s => s.SpelerId == speler.SpelerId), speler.TeamNaam),
            MapSeasonStats(seizoen, wedstrijdStats.Where(s => s.SpelerId == speler.SpelerId)),
            BuildEvents(wedstrijd, wedstrijdStats, speler.SpelerId),
            BuildTeamStats(wedstrijdStats));
    }

    public async Task<SpelerStatsData?> GetSpelerStatsAsync(CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return null;
        }

        var seizoen = await GetSpelerSeizoenAsync(speler.SpelerId, cancellationToken);
        var stats = await GetStatsVoorSpelerAsync(speler.SpelerId, cancellationToken);
        var wedstrijden = await GetSpelerWedstrijdenAsync(speler, cancellationToken);

        var rows = wedstrijden
            .Select(w => MapWedstrijdStatsRow(w, stats.FirstOrDefault(s => s.WedstrijdId == w.WedstrijdId)))
            .OrderByDescending(w => w.Datum)
            .ToList();

        var maanden = stats
            .Join(wedstrijden, s => s.WedstrijdId, w => w.WedstrijdId, (s, w) => new { Stat = s, Wedstrijd = w })
            .GroupBy(x => x.Wedstrijd.Datum.ToString("MMM").ToLowerInvariant())
            .Select(g => new SpelerMaandStats(g.Key, g.Sum(x => x.Stat.Goals ?? 0), g.Sum(x => x.Stat.Assists ?? 0)))
            .ToList();

        return new SpelerStatsData(
            speler,
            MapSeasonStats(seizoen, stats),
            maanden,
            BuildOnderscheidingen(seizoen),
            rows);
    }

    public async Task<SpelerProfielData?> GetSpelerProfielAsync(CancellationToken cancellationToken = default)
    {
        var speler = await GetHuidigeSpelerAsync(cancellationToken);
        if (speler is null)
        {
            return null;
        }

        var seizoen = await GetSpelerSeizoenAsync(speler.SpelerId, cancellationToken);
        var stats = await GetStatsVoorSpelerAsync(speler.SpelerId, cancellationToken);
        return new SpelerProfielData(speler, MapSeasonStats(seizoen, stats));
    }

    private async Task<List<AanwezigheidResponse>> GetAanwezighedenVoorSpelerAsync(
        int spelerId,
        CancellationToken cancellationToken)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<AanwezigheidResponse>>(
            $"api/aanwezigheid/speler/{spelerId}",
            cancellationToken) ?? new();
    }

    private async Task<List<StatistiekResponse>> GetStatsVoorSpelerAsync(
        int spelerId,
        CancellationToken cancellationToken)
    {
        return await _api.GetFromJsonAuthorizedAsync<List<StatistiekResponse>>(
            $"api/spelers/{spelerId}/stats",
            cancellationToken) ?? new();
    }

    private async Task<SeizoenSpelerStats?> GetSpelerSeizoenAsync(
        int spelerId,
        CancellationToken cancellationToken)
    {
        return await _api.GetFromJsonAuthorizedAsync<SeizoenSpelerStats>(
            $"api/spelers/{spelerId}/seizoen",
            cancellationToken);
    }

    private async Task<List<WedstrijdDetailResponse>> GetSpelerWedstrijdenAsync(
        SpelerIdentity speler,
        CancellationToken cancellationToken)
    {
        var wedstrijden = await GetWedstrijdenVoorTeamAsync(speler.TeamId, cancellationToken);
        var details = new List<WedstrijdDetailResponse>();

        foreach (var wedstrijd in wedstrijden)
        {
            var detail = await _api.GetFromJsonAuthorizedAsync<WedstrijdDetailResponse>(
                $"api/wedstrijden/{wedstrijd.WedstrijdId}",
                cancellationToken);

            if (detail is not null)
            {
                details.Add(detail);
            }
        }

        return details;
    }

    private static SpelerSeasonStats MapSeasonStats(
        SeizoenSpelerStats? seizoen,
        IEnumerable<StatistiekResponse> fallbackStats)
    {
        var stats = fallbackStats.ToList();
        return new SpelerSeasonStats(
            seizoen?.TotaalGoals ?? stats.Sum(s => s.Goals ?? 0),
            seizoen?.TotaalAssists ?? stats.Sum(s => s.Assists ?? 0),
            seizoen?.TotaalMinuten ?? stats.Sum(s => s.GespeeldeMinuten ?? 0),
            seizoen?.TotaalGeleKaarten ?? stats.Sum(s => s.GeleKaarten ?? 0),
            seizoen?.TotaalRodeKaarten ?? stats.Sum(s => s.RodeKaarten ?? 0),
            seizoen?.AantalWedstrijden ?? stats.Count);
    }

    private static SpelerTrainingCard MapTraining(TrainingResponse training, AanwezigheidResponse? aanwezigheid)
    {
        return new SpelerTrainingCard(
            training.TrainingId,
            training.Datum,
            training.StartTijd,
            training.EindTijd,
            training.TerreinNaam,
            training.Status.ToString(),
            aanwezigheid?.Aanwezig);
    }

    private static SpelerWedstrijdCard MapWedstrijd(
        WedstrijdDetailResponse wedstrijd,
        IEnumerable<StatistiekResponse> eigenStats,
        string teamNaam)
    {
        var stats = eigenStats.ToList();
        return new SpelerWedstrijdCard(
            wedstrijd.WedstrijdId,
            wedstrijd.Datum,
            wedstrijd.StartTijd,
            string.IsNullOrWhiteSpace(wedstrijd.TeamNaam) ? teamNaam : wedstrijd.TeamNaam,
            wedstrijd.Tegenstander,
            wedstrijd.TerreinNaam,
            wedstrijd.Status.ToString(),
            wedstrijd.ThuisScore,
            wedstrijd.UitScore,
            stats.Sum(s => s.Goals ?? 0),
            stats.Sum(s => s.Assists ?? 0));
    }

    private static SpelerWedstrijdStatsRow MapWedstrijdStatsRow(
        WedstrijdDetailResponse wedstrijd,
        StatistiekResponse? stats)
    {
        var uitslag = wedstrijd.ThuisScore is null || wedstrijd.UitScore is null
            ? wedstrijd.Status.ToString()
            : $"{wedstrijd.ThuisScore}-{wedstrijd.UitScore}";

        var badgeClass = wedstrijd.ThuisScore > wedstrijd.UitScore ? "badge-gespeeld"
            : wedstrijd.ThuisScore == wedstrijd.UitScore ? "badge-afwacht"
            : "badge-geweigerd";

        return new SpelerWedstrijdStatsRow(
            wedstrijd.Datum,
            wedstrijd.Tegenstander,
            uitslag,
            badgeClass,
            stats?.GespeeldeMinuten ?? 0,
            stats?.Goals ?? 0,
            stats?.Assists ?? 0,
            stats?.GeleKaarten ?? 0);
    }

    private static IReadOnlyList<SpelerWedstrijdEvent> BuildEvents(
        WedstrijdDetailResponse wedstrijd,
        IReadOnlyList<StatistiekResponse> stats,
        int spelerId)
    {
        var events = stats
            .Where(s => (s.Goals ?? 0) > 0)
            .Select(s => new SpelerWedstrijdEvent(
                0,
                "G",
                string.Empty,
                $"{s.SpelerNaam} scoorde {s.Goals} keer",
                s.SpelerId == spelerId))
            .ToList();

        if (events.Count == 0)
        {
            events.Add(new SpelerWedstrijdEvent(
                0,
                "i",
                string.Empty,
                wedstrijd.ThuisScore is null ? "Deze wedstrijd is nog niet gespeeld." : "Er zijn nog geen wedstrijdmomenten geregistreerd.",
                false));
        }

        return events;
    }

    private static IReadOnlyList<SpelerTeamStat> BuildTeamStats(IReadOnlyList<StatistiekResponse> stats)
    {
        return new[]
        {
            new SpelerTeamStat("Goals", stats.Sum(s => s.Goals ?? 0), 0),
            new SpelerTeamStat("Assists", stats.Sum(s => s.Assists ?? 0), 0),
            new SpelerTeamStat("Minuten", stats.Sum(s => s.GespeeldeMinuten ?? 0), 0)
        };
    }

    private static IReadOnlyList<SpelerOnderscheiding> BuildOnderscheidingen(SeizoenSpelerStats? seizoen)
    {
        if (seizoen is null)
        {
            return Array.Empty<SpelerOnderscheiding>();
        }

        var items = new List<SpelerOnderscheiding>();
        if (seizoen.TotaalGoals > 0)
        {
            items.Add(new SpelerOnderscheiding("G", "Goals gemaakt", $"{seizoen.TotaalGoals} goals dit seizoen"));
        }

        if (seizoen.TotaalAssists > 0)
        {
            items.Add(new SpelerOnderscheiding("A", "Assists gegeven", $"{seizoen.TotaalAssists} assists dit seizoen"));
        }

        return items;
    }

    private sealed class ClubTeamListItem
    {
        public int TeamId { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string Seizoen { get; set; } = string.Empty;
    }
}
