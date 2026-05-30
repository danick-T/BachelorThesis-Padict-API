# Build-notities CoachAssist

## Thesisfocus

De thesisapplicatie focust op de webapplicatie en de API. Deze delen vormen samen de werkende CoachAssist-toepassing voor de demo en de belangrijkste gebruikersflows.

Het MAUI/mobile-project staat nog in de solution, maar wordt niet gebruikt als hoofdplatform voor de thesisdemo. Dit project kan later verder worden uitgewerkt als mobiele uitbreiding.

## Aanbevolen buildroute

Gebruik deze route om alleen de webapplicatie, API en gedeelde projecten te bouwen:

```powershell
dotnet build CoachAssist.WebApi.slnf --no-restore
```

Deze solution filter bevat:

- `CoachAssist.Api`
- `CoachAssist.Web`
- `CoachAssist.Core`
- `CoachAssist.Data`
- `CoachAssist.Shared`

Los bouwen kan ook:

```powershell
dotnet build CoachAssist\CoachAssist.Web\CoachAssist.Web.csproj --no-restore
dotnet build CoachAssist\CoachAssist.Api\CoachAssist.Api.csproj --no-restore
```

## Waarom de volledige solution kan falen

De volledige solution bevat ook het MAUI/mobile-project:

```powershell
dotnet build CoachAssist.sln --no-restore
```

In deze omgeving faalt die build op het MAUI-project, omdat .NET de Android, iOS, MacCatalyst en Windows SDK-locaties probeert te lezen. De fout verwijst naar beperkte toegang tot:

```text
C:\Users\Pc\AppData\Local\Microsoft SDKs
```

Dit probleem raakt de webapplicatie en API niet. Daarom is de Web/API-buildroute de praktische route voor de thesisdemo.

## Bewust buiten scope

- Geen refactor van het MAUI/mobile-project.
- Geen installatie of aanpassing van Android/iOS/MacCatalyst SDK's.
- Geen wijziging aan de bestaande solution-structuur, zodat andere agents niet worden gehinderd.
