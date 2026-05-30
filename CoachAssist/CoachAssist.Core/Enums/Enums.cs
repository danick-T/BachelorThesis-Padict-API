namespace CoachAssist.Core.Enums;

using NpgsqlTypes;

public enum GebruikerRol
{
    Admin,
    Coach,
    Speler
}

public enum AanvraagStatus
{
    [PgName("in afwachting")]
    InAfwachting,

    [PgName("goedgekeurd")]
    Goedgekeurd,

    [PgName("geweigerd")]
    Geweigerd
}

public enum WedstrijdStatus
{
    [PgName("gepland")]
    Gepland,

    [PgName("gespeeld")]
    Gespeeld,

    [PgName("uitgesteld")]
    Uitgesteld,

    [PgName("geannuleerd")]
    Geannuleerd
}

public enum TrainingStatus
{
    [PgName("gepland")]
    Gepland,

    [PgName("afgelast")]
    Afgelast,

    [PgName("uitgesteld")]
    Uitgesteld
}

public enum VastePositie
{
    [PgName("Keeper")]
    Keeper,

    [PgName("Verdediger")]
    Verdediger,

    [PgName("Middenvelder")]
    Middenvelder,

    [PgName("Aanvaller")]
    Aanvaller,

    [PgName("Niet gespecificeerd")]
    NietGespecificeerd
}

public enum OpstellingPositie
{
    [PgName("Keeper")]
    Keeper,

    [PgName("Rechtsback")]
    Rechtsback,

    [PgName("Linksback")]
    Linksback,

    [PgName("Centrale verdediger")]
    CentraleVerdediger,

    [PgName("Verdedigende middenvelder")]
    VerdedigendeMiddenvelder,

    [PgName("Centrale middenvelder")]
    CentraleMiddenvelder,

    [PgName("Aanvallende middenvelder")]
    AanvallendeMiddenvelder,

    [PgName("Rechtsbuiten")]
    Rechtsbuiten,

    [PgName("Linksbuiten")]
    Linksbuiten,

    [PgName("Spits")]
    Spits,

    [PgName("Schaduwspits")]
    Schaduwspits
}
