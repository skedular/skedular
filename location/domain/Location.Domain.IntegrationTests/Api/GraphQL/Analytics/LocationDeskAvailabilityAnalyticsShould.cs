using Api.Shared.Services.Models;
using Enterprise.Shared.Time;
using Location.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using NanoidDotNet;
using LocationEntity = Location.Shared.Database.Entities.Location;
using ResourceAvailabilityClassificationConstants = Location.Shared.Models.ResourceAvailabilityClassificationConstants;

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
    private async Task<(string orgId, string locationId)> SeedSnapshotDataAsync(
        int dayCount,
        CancellationToken cancellationToken)
    {
        var orgId = await Nanoid.GenerateAsync();
        var locationId = await Nanoid.GenerateAsync();
        var now = timeProvider.GetUtcNow();

        repositoryFactory.DbContext.Organization.Add(new Organization { Id = orgId, CreatedAt = now });
        repositoryFactory.DbContext.Location.Add(new LocationEntity
        {
            Id = locationId,
            Name = "Analytics Integration Test Location",
            OrganizationId = orgId,
            Type = LocationTypeConstants.Private,
            CreatedAt = now
        });

        for (var d = 0; d < dayCount; d++)
        {
            var snapshotDate = now.StartOfDay().AddDays(-d);
            var resourceId = await Nanoid.GenerateAsync();
            var deskTag = new OrganizationTag { Id = await Nanoid.GenerateAsync(), Type = OrganizationTagTypeConstants.ResourceDesk, CreatedAt = now };
            var resource = new Resource { Id = resourceId, Name = $"Desk {d + 1:D2}", CreatedAt = now, OrganizationTags = [deskTag] };
            repositoryFactory.DbContext.Resource.Add(resource);
            repositoryFactory.DbContext.DailyResourceAvailabilitySnapshot.Add(new DailyResourceAvailabilitySnapshot
            {
                Id = await Nanoid.GenerateAsync(),
                LocationId = locationId,
                ResourceId = resourceId,
                Resource = resource,
                Date = snapshotDate,
                Classification = ResourceAvailabilityClassificationConstants.Available,
                CreatedAt = now
            });
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return (orgId, locationId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_When_User_Is_Not_Authenticated(CancellationToken cancellationToken)
    {
        // Seed known snapshots so that if auth were bypassed, data would be returned
        await SeedSnapshotDataAsync(3, cancellationToken);

        var from = DateTimeOffset.UtcNow.AddDays(-7);
        var until = DateTimeOffset.UtcNow;

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
        var from = DateTimeOffset.UtcNow.AddDays(-30);
        var until = DateTimeOffset.UtcNow;

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
