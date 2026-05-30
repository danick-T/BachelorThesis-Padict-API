namespace CoachAssist.Shared.Models.SpelerPages;

public sealed record SpelerIdentity(
    int SpelerId,
    int TeamId,
    string Voornaam,
    string Achternaam,
    string TeamNaam,
    string? ClubNaam,
    string? Positie,
    int? Rugnummer,
    string? Email)
{
    public string VolledigeNaam => $"{Voornaam} {Achternaam}".Trim();
    public string Initialen => string.Concat(new[] { Voornaam, Achternaam }
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(s => char.ToUpperInvariant(s.Trim()[0])));
}

public sealed record SpelerSeasonStats(
    int Goals,
    int Assists,
    int Minuten,
    int GeleKaarten,
    int RodeKaarten,
    int Wedstrijden);

public sealed record SpelerTrainingCard(
    int TrainingId,
    DateTime Datum,
    TimeSpan StartTijd,
    TimeSpan EindTijd,
    string TerreinNaam,
    string Status,
    bool? Aanwezig);

public sealed record SpelerWedstrijdCard(
    int WedstrijdId,
    DateTime Datum,
    TimeSpan StartTijd,
    string TeamNaam,
    string Tegenstander,
    string TerreinNaam,
    string Status,
    int? ThuisScore,
    int? UitScore,
    int Goals,
    int Assists);

public sealed record SpelerDashboardData(
    SpelerIdentity Speler,
    SpelerSeasonStats Stats,
    SpelerTrainingCard? VolgendeTraining,
    SpelerWedstrijdCard? VolgendeWedstrijd,
    IReadOnlyList<SpelerWedstrijdCard> LaatsteWedstrijden,
    int AanwezigTrainingen,
    int TotaalTrainingen);

public enum SpelerSchemaType
{
    Training,
    Wedstrijd
}

public sealed record SpelerSchemaItem(
    int Id,
    SpelerSchemaType Type,
    DateTime Datum,
    TimeSpan StartTijd,
    string Titel,
    string Subtitel,
    string Status);

public sealed record SpelerTrainingAanwezigheidData(
    SpelerTrainingCard Training,
    int Aanwezig,
    int Afwezig,
    int GeenReactie);

public sealed record SpelerWedstrijdEvent(
    int Minuut,
    string Icon,
    string CssClass,
    string Tekst,
    bool IsEigenSpeler);

public sealed record SpelerTeamStat(string Naam, int Links, int Rechts);

public sealed record SpelerWedstrijdDetailData(
    SpelerIdentity Speler,
    SpelerWedstrijdCard Wedstrijd,
    SpelerSeasonStats SpelerStats,
    IReadOnlyList<SpelerWedstrijdEvent> Events,
    IReadOnlyList<SpelerTeamStat> TeamStats);

public sealed record SpelerMaandStats(string Naam, int Goals, int Assists);

public sealed record SpelerOnderscheiding(string Icon, string Naam, string Subtitel);

public sealed record SpelerWedstrijdStatsRow(
    DateTime Datum,
    string Tegenstander,
    string Uitslag,
    string BadgeClass,
    int Minuten,
    int Goals,
    int Assists,
    int GeleKaarten);

public sealed record SpelerStatsData(
    SpelerIdentity Speler,
    SpelerSeasonStats Totaal,
    IReadOnlyList<SpelerMaandStats> Maanden,
    IReadOnlyList<SpelerOnderscheiding> Onderscheidingen,
    IReadOnlyList<SpelerWedstrijdStatsRow> Wedstrijden);

public sealed record SpelerProfielData(
    SpelerIdentity Speler,
    SpelerSeasonStats Stats);
