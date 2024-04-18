FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV NUGET_PACKAGES="/packages"

WORKDIR "/src"
COPY . .
WORKDIR "/src/all-in-one/AllApis"
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet restore && dotnet tool restore
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet build --no-restore "AllApis.csproj" -c Release -o /app/build

FROM build AS test
RUN mkdir -p /coverage/reports

FROM test AS publish
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet publish "AllApis.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
RUN groupadd --gid 2000 unityhubio && useradd --uid 1000 --gid 2000 unityhubio
RUN chown unityhubio:unityhubio /app
USER unityhubio:unityhubio 

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AllApis.dll"]
