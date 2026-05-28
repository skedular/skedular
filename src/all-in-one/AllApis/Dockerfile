FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ENV NUGET_PACKAGES="/packages"

WORKDIR "/src"
COPY . .
WORKDIR "/src/src/all-in-one/AllApis"
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet restore && dotnet tool restore
# RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet build --no-restore "AllApis.csproj" -c Release -o /app/build

FROM build AS test
RUN mkdir -p /coverage/reports

# Verify gateway.far is a valid, fully readable zip before publishing.
# A truncated or corrupted gateway.far causes the Fusion gateway to fail
# at startup (502 on all GraphQL requests) with no obvious error message.
# Catching it here aborts the build immediately with a clear failure.
FROM build AS gateway-verify
RUN apt-get update -y && apt-get install -y --no-install-recommends unzip \
    && bash /src/.github/actions/build-test-push/verify-gateway-far.sh \
    /src/src/gateway/apis/Gateway/gateway.far \
    true

FROM gateway-verify AS publish
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet publish "AllApis.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

RUN apt-get update && apt-get install -y curl && apt-get clean && rm -rf /var/lib/apt/lists/*
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AllApis.dll"]
