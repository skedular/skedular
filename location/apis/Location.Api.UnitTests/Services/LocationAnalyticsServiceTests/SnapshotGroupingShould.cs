using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using LocationEntity = Location.Shared.Database.Entities.Location;
using RoomCountRecordingEntity = Location.Shared.Database.Entities.DailyRoomCountRecording;
using ResourceAvailabilityClassificationConstants = Location.Shared.Models.ResourceAvailabilityClassificationConstants;

namespace Location.Api.UnitTests.Services.LocationAnalyticsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SnapshotGroupingShould
{
    private static LocationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LocationDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString())
            .Options;
        return new LocationDbContext(options, new CustomDbContextOptions<LocationDbContext> { IsPooled = false });
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Groups_Snapshots_By_Date_With_Correct_Counts(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyDeskCountRecordingRepository deskCountRepository,
        [Frozen] IDailyRoomCountRecordingRepository roomCountRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        // Arrange
        const string LocationId = "loc-snap";
        const string OrgId = "org-snap";
        const string CustomerId = "cust-snap";

        await using var dbContext = CreateInMemoryContext();

        var location = new LocationEntity { Id = LocationId, Name = "Snap Office", OrganizationId = OrgId };
        var from = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero);
        var day1 = from;
        var day2 = from.AddDays(1);

        var deskTag = new OrganizationTag { Id = "tag-desk", Type = OrganizationTagTypeConstants.ResourceDesk };

        // Two snapshots on day1, one on day2
        var snapshots = new[]
        {
            new DailyResourceAvailabilitySnapshot
            {
                Id = "snap-1",
                Location = location,
                LocationId = LocationId,
                ResourceId = "res-a",
                Resource = new Resource { Id = "res-a", Name = "Desk A", OrganizationTags = [deskTag] },
                Date = day1,
                Classification = ResourceAvailabilityClassificationConstants.Available
            },
            new DailyResourceAvailabilitySnapshot
            {
                Id = "snap-2",
                Location = location,
                LocationId = LocationId,
                ResourceId = "res-b",
                Resource = new Resource { Id = "res-b", Name = "Desk B", OrganizationTags = [deskTag] },
                Date = day1,
                Classification = ResourceAvailabilityClassificationConstants.Booked
            },
            new DailyResourceAvailabilitySnapshot
            {
                Id = "snap-3",
                Location = location,
                LocationId = LocationId,
                ResourceId = "res-c",
                Resource = new Resource { Id = "res-c", Name = "Desk C", OrganizationTags = [deskTag] },
                Date = day2,
                Classification = ResourceAvailabilityClassificationConstants.Unavailable
            }
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.DailyRoomCountRecordingRepository).Returns(roomCountRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);

        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(CustomerId);
        A.CallTo(() => organizationAuthorizationService.CanViewAnalyticsAsync(OrgId, CustomerId, A<CancellationToken>._))
            .Returns(true);

        A.CallTo(() => deskCountRepository.GetByLocationIdsAndDateRangeAsync(A<IReadOnlyList<string>>._, from, until, A<CancellationToken>._))
            .Returns([]);

        A.CallTo(() => roomCountRepository.GetByLocationIdsAndDateRangeAsync(
                A<ICollection<string>>._, from, until, A<CancellationToken>._))
            .Returns(Array.Empty<RoomCountRecordingEntity>());

        A.CallTo(() => snapshotRepository.GetByLocationIdAndDateRangeAsync(
                LocationId, from, until, A<string?>._, A<CancellationToken>._))
            .Returns(snapshots);

        var sut = new LocationAnalyticsService(
            repositoryFactory, locationService, cachedCustomerService, organizationAuthorizationService);

        // Act
        var result = await sut.GetAnalyticsAsync(LocationId, from, until, cancellationToken);

        // Assert – 2 distinct date groups
        result.ResourceAvailabilitySnapshots.Count.ShouldBe(2);

        var day1Report = result.ResourceAvailabilitySnapshots.Single(r => r.Date == day1);
        day1Report.AvailableCount.ShouldBe(1);
        day1Report.BookedCount.ShouldBe(1);
        day1Report.UnavailableCount.ShouldBe(0);

        var day2Report = result.ResourceAvailabilitySnapshots.Single(r => r.Date == day2);
        day2Report.UnavailableCount.ShouldBe(1);
        day2Report.AvailableCount.ShouldBe(0);
        day2Report.BookedCount.ShouldBe(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Day_With_No_Snapshot_Is_Not_In_Result(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyDeskCountRecordingRepository deskCountRepository,
        [Frozen] IDailyRoomCountRecordingRepository roomCountRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        // Arrange – snapshot repository returns nothing for this location/range
        const string LocationId = "loc-no-snap";
        const string OrgId = "org-no-snap";
        const string CustomerId = "cust-no-snap";

        await using var dbContext = CreateInMemoryContext();
        var location = new LocationEntity { Id = LocationId, Name = "No Snap Office", OrganizationId = OrgId };
        var from = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 4, 7, 0, 0, 0, TimeSpan.Zero);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.DailyRoomCountRecordingRepository).Returns(roomCountRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);

        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(CustomerId);
        A.CallTo(() => organizationAuthorizationService.CanViewAnalyticsAsync(OrgId, CustomerId, A<CancellationToken>._)).Returns(true);
        A.CallTo(() => deskCountRepository.GetByLocationIdsAndDateRangeAsync(A<IReadOnlyList<string>>._, from, until, A<CancellationToken>._))
            .Returns([]);

        A.CallTo(() => roomCountRepository.GetByLocationIdsAndDateRangeAsync(A<ICollection<string>>._, from, until, A<CancellationToken>._))
            .Returns(Array.Empty<RoomCountRecordingEntity>());

        A.CallTo(() => snapshotRepository.GetByLocationIdAndDateRangeAsync(LocationId, from, until, A<string?>._, A<CancellationToken>._))
            .Returns([]);

        var sut = new LocationAnalyticsService(
            repositoryFactory, locationService, cachedCustomerService, organizationAuthorizationService);

        // Act
        var result = await sut.GetAnalyticsAsync(LocationId, from, until, cancellationToken);

        // Assert
        result.ResourceAvailabilitySnapshots.ShouldBeEmpty();
    }
}
