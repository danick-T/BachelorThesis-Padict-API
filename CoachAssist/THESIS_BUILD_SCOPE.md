# Build-scope voor thesis

Voor de thesis ligt de focus op de webapp en de API.

Deze projecten bouwen succesvol:

```text
dotnet build CoachAssist.Api\CoachAssist.Api.csproj --no-restore
dotnet build CoachAssist.Web\CoachAssist.Web.csproj --no-restore
```

De volledige solution bevat ook het MAUI/mobile-project `CoachAssist`.
Dat project vraagt lokale platform-SDK's op via `C:\Users\Pc\AppData\Local\Microsoft SDKs`.
Op deze machine geeft die map een toegangsprobleem, waardoor de volledige solution-build faalt op Android, iOS, MacCatalyst en Windows SDK-detectie.

Dit is daarom afgebakend als buiten de hoofdscope van de thesis.
De in te dienen en te demonstreren applicatie bestaat uit:

- `CoachAssist.Api`
- `CoachAssist.Web`
- `CoachAssist.Shared`
- `CoachAssist.Core`
- `CoachAssist.Data`

Het MAUI/mobile-project kan later opnieuw worden opgepakt wanneer de lokale SDK-toegang en workloads correct staan.
