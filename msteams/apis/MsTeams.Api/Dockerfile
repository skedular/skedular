FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ENV NUGET_PACKAGES="/packages"

WORKDIR "/src"
COPY . .
WORKDIR "/src/msteams/apis/MsTeams.Api"
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet restore && dotnet tool restore
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet build --no-restore "MsTeams.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet publish "MsTeams.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

RUN apt-get update && apt-get install -y curl && apt-get clean && rm -rf /var/lib/apt/lists/*

WORKDIR /app
RUN groupadd --gid 2000 unityhubio && useradd --uid 1000 --gid 2000 unityhubio
RUN chown unityhubio:unityhubio /app
USER unityhubio:unityhubio

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MsTeams.Api.dll"]
