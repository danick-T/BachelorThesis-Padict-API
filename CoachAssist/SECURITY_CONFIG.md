# Security-configuratie

De API bevat geen echte databasegegevens of JWT-secret meer in de repository.
Deze waarden moeten lokaal of in productie buiten de broncode worden gezet.

## Lokale setup

Zet de databaseverbinding via user secrets of via een environment variable met de naam:

```text
ConnectionStrings__DefaultConnection
```

Zet de JWT-secret via een van deze opties:

```text
JwtSettings__SecretKey
COACHASSIST_JWT_SECRET
```

Gebruik voor de JWT-secret een lange willekeurige waarde. Gebruik niet dezelfde waarde voor lokaal testen en productie.

## Publicatie

Voor GitHub of een andere publieke omgeving mogen databasegegevens, wachtwoorden en tokens niet in `appsettings.json` of `appsettings.Development.json` staan.
De server of hostingomgeving moet deze waarden zelf injecteren als environment variables.
