FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CoachAssist/ ./CoachAssist/
RUN dotnet restore CoachAssist/CoachAssist.Api/CoachAssist.Api.csproj
RUN dotnet publish CoachAssist/CoachAssist.Api/CoachAssist.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

CMD ["sh", "-c", "dotnet CoachAssist.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
