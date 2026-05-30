# Task3_FlowTest status

## Seedaccounts

Alle seedaccounts gebruiken het wachtwoord `SeedPass2026!`.

- Admin: `admin@coachassist.test`
- Coach U17: `coach.u17@coachassist.test`
- Speler U17: `speler.u17.01@coachassist.test`

Deze accounts horen bij club `KFC Beersel-Drogenbos` en seizoen `2025-2026`.

## Flowcheck

| Stap | Status | Opmerking |
| --- | --- | --- |
| Speler registreert | Gefixt | Register + rolkeuze maakt nu echt een account aan via `api/auth/register`. |
| Speler kiest club | Gefixt | Clubkeuze laadt nu clubs uit `api/clubs` in plaats van vaste demo-data. |
| Speler kiest team | Gefixt | Teamkeuze laadt nu teams uit `api/clubs/{id}/teams`. |
| Speler vraagt lidmaatschap aan | OK | De onboarding verstuurt een clubaanvraag via `api/aanvragen/club`. |
| Admin keurt clubaanvraag goed | OK | `/admin/aanvragen` gebruikt `api/aanvragen/club/{id}/goedkeuren`. |
| Speler vraagt team aan | Gefixt | `api/aanvragen/team` maakt nu automatisch een basis-spelerprofiel aan als dat nog ontbreekt. |
| Coach keurt teamaanvraag goed | OK | `/coach/verzoeken` gebruikt `api/aanvragen/team/{id}/goedkeuren`. |
| Coach plant training | OK | `/coach/training/nieuw` gebruikt `api/trainingen`. |
| Coach plant wedstrijd | OK | `/coach/wedstrijd/nieuw` gebruikt `api/wedstrijden`. |
| Speler ziet schema | OK | `/speler/schema` gebruikt trainingen en wedstrijden van het gekoppelde team. |
| Speler geeft aanwezigheid door | OK | `/speler/aanwezigheid/{id}` gebruikt `api/aanwezigheid`. |
| Coach vult stats in | OK | `/coach/stats-invoeren/{id}` gebruikt `api/statistieken/wedstrijd/{id}`. |

## Verificatie

- `dotnet build CoachAssist.Api\CoachAssist.Api.csproj --no-restore`: geslaagd.
- `dotnet build CoachAssist.Web\CoachAssist.Web.csproj --no-restore`: geslaagd.

## Openstaand

- De flow is via code en build gecontroleerd. Er is geen volledige live browsertest uitgevoerd tegen een draaiende database.
- De onboarding werkt in twee stappen: eerst clubaanvraag, daarna na admin-goedkeuring de teamaanvraag. Dit past bij de huidige API-regels.
