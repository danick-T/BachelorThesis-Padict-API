using CoachAssist.Core.Entities;
using CoachAssist.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Gebruiker> Gebruikers => Set<Gebruiker>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Terrein> Terreinen => Set<Terrein>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Speler> Spelers => Set<Speler>();
    public DbSet<ClubAanvraag> ClubAanvragen => Set<ClubAanvraag>();
    public DbSet<TeamAanvraag> TeamAanvragen => Set<TeamAanvraag>();
    public DbSet<Wedstrijd> Wedstrijden => Set<Wedstrijd>();
    public DbSet<SpelerStatistiek> SpelerStatistieken => Set<SpelerStatistiek>();
    public DbSet<Opstelling> Opstellingen => Set<Opstelling>();
    public DbSet<OpstellingSpeler> OpstellingSpelers => Set<OpstellingSpeler>();
    public DbSet<Training> Trainingen => Set<Training>();
    public DbSet<TrainingAanwezigheid> TrainingAanwezigheden => Set<TrainingAanwezigheid>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresEnum<GebruikerRol>("gebruikersrol");
        builder.HasPostgresEnum<AanvraagStatus>("aanvraagstatus");
        builder.HasPostgresEnum<WedstrijdStatus>("wedstrijdstatus");
        builder.HasPostgresEnum<TrainingStatus>("trainingstatus");
        builder.HasPostgresEnum<VastePositie>("vaste_positie");
        builder.HasPostgresEnum<OpstellingPositie>("opstelling_positie");

        builder.Entity<Club>().ToTable("club");
        builder.Entity<Gebruiker>().ToTable("gebruiker");
        builder.Entity<Terrein>().ToTable("terrein");
        builder.Entity<Team>().ToTable("team");
        builder.Entity<Speler>().ToTable("speler");
        builder.Entity<ClubAanvraag>().ToTable("clubaanvraag");
        builder.Entity<TeamAanvraag>().ToTable("teamaanvraag");
        builder.Entity<Wedstrijd>().ToTable("wedstrijd");
        builder.Entity<SpelerStatistiek>().ToTable("spelerstatistiek");
        builder.Entity<Opstelling>().ToTable("opstelling");
        builder.Entity<OpstellingSpeler>().ToTable("opstellingspeler");
        builder.Entity<Training>().ToTable("training");
        builder.Entity<TrainingAanwezigheid>().ToTable("trainingaanwezigheid");

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }

        builder.Entity<Terrein>().HasKey(t => t.TerreinId);
        builder.Entity<ClubAanvraag>().HasKey(a => a.AanvraagId);
        builder.Entity<TeamAanvraag>().HasKey(a => a.AanvraagId);
        builder.Entity<TrainingAanwezigheid>().HasKey(a => a.AanwezigheidId);
        builder.Entity<SpelerStatistiek>().HasKey(s => s.StatistiekId);

        builder.Entity<Gebruiker>().HasIndex(g => g.Email).IsUnique();
        builder.Entity<Club>().HasIndex(c => c.Naam).IsUnique();
        builder.Entity<Team>().HasIndex(t => new { t.Naam, t.Seizoen, t.ClubId }).IsUnique();
        builder.Entity<Terrein>().HasIndex(t => new { t.Naam, t.ClubId }).IsUnique();
        builder.Entity<TeamAanvraag>().HasIndex(a => new { a.SpelerId, a.TeamId }).IsUnique();
        builder.Entity<ClubAanvraag>().HasIndex(a => new { a.GebruikerId, a.ClubId }).IsUnique();
        builder.Entity<OpstellingSpeler>().HasIndex(o => new { o.OpstellingId, o.SpelerId }).IsUnique();
        builder.Entity<SpelerStatistiek>().HasIndex(s => new { s.SpelerId, s.WedstrijdId }).IsUnique();
        builder.Entity<TrainingAanwezigheid>().HasIndex(a => new { a.TrainingId, a.SpelerId }).IsUnique();
        builder.Entity<Opstelling>().HasIndex(o => o.WedstrijdId).IsUnique();

        builder.Entity<Team>()
            .HasOne(t => t.Coach)
            .WithMany(g => g.GecoachteTeams)
            .HasForeignKey(t => t.CoachId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Speler>()
            .HasOne(s => s.Gebruiker)
            .WithOne(g => g.Speler)
            .HasForeignKey<Speler>(s => s.GebruikerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Speler>()
            .HasOne(s => s.Team)
            .WithMany(t => t.Spelers)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Wedstrijd>()
            .HasOne(w => w.Terrein)
            .WithMany(t => t.Wedstrijden)
            .HasForeignKey(w => w.TerreinId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Training>()
            .HasOne(t => t.Terrein)
            .WithMany(t => t.Trainingen)
            .HasForeignKey(t => t.TerreinId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Opstelling>()
            .HasOne(o => o.Wedstrijd)
            .WithOne(w => w.Opstelling)
            .HasForeignKey<Opstelling>(o => o.WedstrijdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Gebruiker>().Property(g => g.Rol).HasColumnType("gebruikersrol");
        builder.Entity<ClubAanvraag>().Property(a => a.Status).HasColumnType("aanvraagstatus");
        builder.Entity<TeamAanvraag>().Property(a => a.Status).HasColumnType("aanvraagstatus");
        builder.Entity<Wedstrijd>().Property(w => w.Status).HasColumnType("wedstrijdstatus");
        builder.Entity<Training>().Property(t => t.Status).HasColumnType("trainingstatus");
        builder.Entity<Speler>().Property(s => s.VastePositie).HasColumnType("vaste_positie");
        builder.Entity<OpstellingSpeler>().Property(o => o.PositieNaam).HasColumnType("opstelling_positie");

        builder.Entity<Wedstrijd>().Property(w => w.Datum).HasColumnType("date");
        builder.Entity<Training>().Property(t => t.Datum).HasColumnType("date");
        builder.Entity<Wedstrijd>().Property(w => w.StartTijd).HasColumnType("time without time zone");
        builder.Entity<Training>().Property(t => t.StartTijd).HasColumnType("time without time zone");
        builder.Entity<Training>().Property(t => t.EindTijd).HasColumnType("time without time zone");
    }
}
