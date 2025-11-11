FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ENV NUGET_PACKAGES="/packages"

WORKDIR "/src"
COPY . .
WORKDIR "/src/booking/apis/Booking.Api"
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet restore && dotnet tool restore
# RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet build --no-restore "Booking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,target=${NUGET_PACKAGES} dotnet publish "Booking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

RUN apt-get update && apt-get install -y curl && apt-get clean && rm -rf /var/lib/apt/lists/*
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Api.dll"]
