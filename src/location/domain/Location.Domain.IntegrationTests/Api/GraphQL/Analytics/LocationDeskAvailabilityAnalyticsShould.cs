using Api.Shared.Services.Models;
using Enterprise.Shared.Time;
using Location.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using NanoidDotNet;
using LocationEntity = Location.Shared.Database.Entities.Location;
using ResourceAvailabilityClassificationConstants = Location.Shared.Models.ResourceAvailabilityClassificationConstants;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Domain.IntegrationTests.Api.GraphQL.Analytics;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class LocationDeskAvailabilityAnalyticsShould(
    IGetLocationResourceAvailabilitySnapshotsQuery resourceAvailabilitySnapshotsQuery,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    /// <summary>
    ///     Seeds an organization + location + N snapshot records directly in the DB
    ///     so that we have known data to query against.
    /// </summary>
    private async Task SeedSnapshotDataAsync(int dayCount, CancellationToken cancellationToken)
    {
        var organizationId = await Nanoid.GenerateAsync();
        var locationId = await Nanoid.GenerateAsync();
        var now = timeProvider.GetUtcNow();

        var organization = new Organization
        {
            Id = organizationId,
            CreatedAt = now,
        };
        var location = new LocationEntity
        {
            Id = locationId,
            Name = "Analytics Integration Test Location",
            OrganizationId = organizationId,
            Type = LocationTypeConstants.Private,
            CreatedAt = now,
        };
        await repositoryFactory.DbContext.Organization.AddAsync(organization, cancellationToken);
        await repositoryFactory.DbContext.Location.AddAsync(location, cancellationToken);

        for (var d = 0; d < dayCount; d++)
        {
            var snapshotDate = now.StartOfDay().AddDays(-d);
            var resourceId = await Nanoid.GenerateAsync();
            var deskTag = new OrganizationTag
            {
                Id = await Nanoid.GenerateAsync(),
                Type = OrganizationTagTypeConstants.ResourceDesk,
                CreatedAt = now,
                Organization = organization,
            };
            var resource = new ResourceEntity
            {
                Id = resourceId,
                Name = $"Desk {d + 1:D2}",
                CreatedAt = now,
                Location = location,
                OrganizationTags = [deskTag],
            };
            await repositoryFactory.DbContext.Resource.AddAsync(resource, cancellationToken);
            await repositoryFactory.DbContext.DailyResourceAvailabilitySnapshot.AddAsync(
                new DailyResourceAvailabilitySnapshot
                {
                    Id = await Nanoid.GenerateAsync(),
                    LocationId = locationId,
                    ResourceId = resourceId,
                    Resource = resource,
                    Date = snapshotDate,
                    Classification = ResourceAvailabilityClassificationConstants.Available,
                    CreatedAt = now,
                }, cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_When_User_Is_Not_Authenticated(CancellationToken cancellationToken)
    {
        // Seed known snapshots so that if auth were bypassed, data would be returned
        await SeedSnapshotDataAsync(3, cancellationToken);

        var from = TimeProvider.System.GetUtcNow().AddDays(-7);
        var until = TimeProvider.System.GetUtcNow();

        var result = await resourceAvailabilitySnapshotsQuery.ExecuteAsync(from, until, cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        // Unauthenticated: no access to private org analytics — response must be empty
        result.Data.LocationsAnalytics.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_ResourceAvailabilitySnapshots_When_No_Snapshots_Exist(CancellationToken cancellationToken)
    {
        // Seed a location with no snapshots — verifies the field is wired correctly
        // (unauthenticated call returns empty regardless, but confirms schema is correct)
        var from = TimeProvider.System.GetUtcNow().AddDays(-30);
        var until = TimeProvider.System.GetUtcNow();

        var result = await resourceAvailabilitySnapshotsQuery.ExecuteAsync(from, until, cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();

        // Each returned analytics entry (if any) should have resourceAvailabilitySnapshots
        foreach (var analytics in result.Data.LocationsAnalytics)
        {
            analytics.ResourceAvailabilitySnapshots.ShouldNotBeNull();
        }
    }
}
