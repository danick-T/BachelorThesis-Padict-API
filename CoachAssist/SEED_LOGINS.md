# CoachAssist Seed Logins

Deze logins horen bij `Seed/coachassist_seed_data.sql`.

Belangrijk: het seedbestand zet `PasswordHash` op een echte ASP.NET Identity hash van `SeedPass2026!`. Daardoor kunnen deze accounts meteen via `/api/auth/login` gebruikt worden.

## Algemene gegevens

- Club: KFC Beersel-Drogenbos
- Seizoen: 2025-2026
- Wachtwoord voor alle seedaccounts: `SeedPass2026!`

## Admin

| Rol | Naam | E-mail | Wachtwoord |
| --- | --- | --- | --- |
| Admin | Sofie Vermeiren | `admin@coachassist.test` | `SeedPass2026!` |

## Coaches

| Team | Coach | E-mail | Wachtwoord |
| --- | --- | --- | --- |
| U13 | Thomas De Smet | `coach.u13@coachassist.test` | `SeedPass2026!` |
| U15 | Karim Benali | `coach.u15@coachassist.test` | `SeedPass2026!` |
| U17 | Jeroen Peeters | `coach.u17@coachassist.test` | `SeedPass2026!` |
| U21 | Niels Van den Broeck | `coach.u21@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Ruben Claes | `coach.eerste@coachassist.test` | `SeedPass2026!` |

## Spelerlogins

Per team hebben de eerste 5 spelers een login. De overige spelers bestaan wel als spelerrecord, maar zonder gebruikersaccount. Dat maakt de data realistischer.

| Team | Speler | E-mail | Wachtwoord |
| --- | --- | --- | --- |
| U13 | Milan Verhaert | `speler.u13.01@coachassist.test` | `SeedPass2026!` |
| U13 | Noah Jacobs | `speler.u13.02@coachassist.test` | `SeedPass2026!` |
| U13 | Luca Peeters | `speler.u13.03@coachassist.test` | `SeedPass2026!` |
| U13 | Mathis De Wolf | `speler.u13.04@coachassist.test` | `SeedPass2026!` |
| U13 | Finn Maes | `speler.u13.05@coachassist.test` | `SeedPass2026!` |
| U15 | Senne Goossens | `speler.u15.01@coachassist.test` | `SeedPass2026!` |
| U15 | Tibo Willems | `speler.u15.02@coachassist.test` | `SeedPass2026!` |
| U15 | Yanis Bakkali | `speler.u15.03@coachassist.test` | `SeedPass2026!` |
| U15 | Arthur Lenaerts | `speler.u15.04@coachassist.test` | `SeedPass2026!` |
| U15 | Adam El Idrissi | `speler.u15.05@coachassist.test` | `SeedPass2026!` |
| U17 | Rayan Messaoudi | `speler.u17.01@coachassist.test` | `SeedPass2026!` |
| U17 | Jules Vercammen | `speler.u17.02@coachassist.test` | `SeedPass2026!` |
| U17 | Mats Van Acker | `speler.u17.03@coachassist.test` | `SeedPass2026!` |
| U17 | Lennert Cools | `speler.u17.04@coachassist.test` | `SeedPass2026!` |
| U17 | Ilyas Bouazza | `speler.u17.05@coachassist.test` | `SeedPass2026!` |
| U21 | Kobe Vranckx | `speler.u21.01@coachassist.test` | `SeedPass2026!` |
| U21 | Niels Segers | `speler.u21.02@coachassist.test` | `SeedPass2026!` |
| U21 | Bram Michiels | `speler.u21.03@coachassist.test` | `SeedPass2026!` |
| U21 | Samir Ait Omar | `speler.u21.04@coachassist.test` | `SeedPass2026!` |
| U21 | Jarne De Ridder | `speler.u21.05@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Wout Vandeputte | `speler.eerste.01@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Glenn Dierckx | `speler.eerste.02@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Olivier Hermans | `speler.eerste.03@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Mehdi Rahmani | `speler.eerste.04@coachassist.test` | `SeedPass2026!` |
| Eerste ploeg | Arne Van Damme | `speler.eerste.05@coachassist.test` | `SeedPass2026!` |
