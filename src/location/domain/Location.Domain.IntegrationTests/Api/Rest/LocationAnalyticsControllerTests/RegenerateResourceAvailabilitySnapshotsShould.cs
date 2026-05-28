using Api.Shared.Clients.OpenApi.Skedular.Location.Analytics.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Time;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using NanoidDotNet;
using LocationEntity = Location.Shared.Database.Entities.Location;

namespace Location.Domain.IntegrationTests.Api.Rest.LocationAnalyticsControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class RegenerateResourceAvailabilitySnapshotsShould(
    ILocationAnalyticsClient locationAnalyticsClient,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    private async Task<string> SeedLocationAsync(CancellationToken cancellationToken)
    {
        var organizationId = await Nanoid.GenerateAsync();
        var locationId = await Nanoid.GenerateAsync();
        var now = timeProvider.GetUtcNow();

        repositoryFactory.DbContext.Organization.Add(new Organization { Id = organizationId, CreatedAt = now });
        repositoryFactory.DbContext.Location.Add(new LocationEntity
        {
            Id = locationId,
            Name = "Backfill Test Location",
            OrganizationId = organizationId,
            Type = LocationTypeConstants.Private,
            CreatedAt = now
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return locationId;
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Ok_For_Valid_Location_And_Date_Range(CancellationToken cancellationToken)
    {
        var locationId = await SeedLocationAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();

        await locationAnalyticsClient.RegenerateResourceAvailabilitySnapshotsAsync(
            locationId,
            new RegenerateResourceAvailabilitySnapshotsInput { From = now.AddDays(-6).StartOfDay(), Until = now.StartOfDay() },
            cancellationToken);

        // The endpoint triggers async Temporal workflows for each day; the call should
        // succeed without exception. Idempotency is verified by calling twice.
        await locationAnalyticsClient.RegenerateResourceAvailabilitySnapshotsAsync(
            locationId,
            new RegenerateResourceAvailabilitySnapshotsInput { From = now.AddDays(-6).StartOfDay(), Until = now.StartOfDay() },
            cancellationToken);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Ok_For_Unknown_Location(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        // Should silently succeed (not error) when the location does not exist
        await locationAnalyticsClient.RegenerateResourceAvailabilitySnapshotsAsync(
            await Nanoid.GenerateAsync(),
            new RegenerateResourceAvailabilitySnapshotsInput { From = now.AddDays(-1).StartOfDay(), Until = now.StartOfDay() },
            cancellationToken);
    }
}
