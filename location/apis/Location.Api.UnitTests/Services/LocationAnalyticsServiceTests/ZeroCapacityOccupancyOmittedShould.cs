using Enterprise.Shared.Database;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using LocationEntity = Location.Shared.Database.Entities.Location;

namespace Location.Api.UnitTests.Services.LocationAnalyticsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ZeroCapacityOccupancyOmittedShould
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
    public async Task Desk_Day_With_Zero_Capacity_Is_Not_In_OccupancyPercentage(
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
        const string LocationId = "loc-zero";
        const string OrgId = "org-1";
        const string CustomerId = "cust-1";

        await using var dbContext = CreateInMemoryContext();

        var location = new LocationEntity { Id = LocationId, Name = "Zero Desk Office", OrganizationId = OrgId };
        var from = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 4, 3, 0, 0, 0, TimeSpan.Zero);

        // Day 1: Count=5 (non-zero) — should appear in result
        // Day 2: Count=0 (zero capacity) — should be omitted
        var deskDay1 = new DailyDeskCountRecording { Id = "ddcr-1", Date = from, Count = 5, Location = location };
        var deskDay2 = new DailyDeskCountRecording { Id = "ddcr-2", Date = from.AddDays(1), Count = 0, Location = location };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.DailyRoomCountRecordingRepository).Returns(roomCountRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);

        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(CustomerId);
        A.CallTo(() => organizationAuthorizationService.CanViewAnalyticsAsync(OrgId, CustomerId, A<CancellationToken>._)).Returns(true);

        // Repository returns both desk count recordings (one zero, one non-zero)
        A.CallTo(() => deskCountRepository.GetByLocationIdsAndDateRangeAsync(
                A<IReadOnlyList<string>>.That.Contains(LocationId), from, until, A<CancellationToken>._))
            .Returns([deskDay1, deskDay2]);

        A.CallTo(() => roomCountRepository.GetByLocationIdsAndDateRangeAsync(
                A<ICollection<string>>.That.Contains(LocationId), from, until, A<CancellationToken>._))
            .Returns(Array.Empty<DailyRoomCountRecording>());

        A.CallTo(() => snapshotRepository.GetByLocationIdAndDateRangeAsync(
                LocationId, from, until, A<string?>._, A<CancellationToken>._))
            .Returns(Array.Empty<DailyResourceAvailabilitySnapshot>());

        // DailyBookingCountRecording (via DbContext) — empty for this test
        // DailyDeskBookingCountRecording — empty
        // DailyRoomBookingCountRecording — empty
        // (in-memory context has nothing seeded)

        var sut = new LocationAnalyticsService(
            repositoryFactory, locationService, cachedCustomerService, organizationAuthorizationService);

        // Act
        var result = await sut.GetAnalyticsAsync(LocationId, from, until, cancellationToken);

        // Assert – only day1 (count=5) appears; day2 (count=0) is omitted
        result.DesksOccupancyPercentage.Count.ShouldBe(1);
        result.DesksOccupancyPercentage.First().Date.ShouldBe(from);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Auth_Fail_Returns_Empty_Analytics(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        // Arrange
        const string LocationId = "loc-noauth";
        const string OrgId = "org-noauth";
        const string CustomerId = "cust-noauth";

        await using var dbContext = CreateInMemoryContext();
        var location = new LocationEntity { Id = LocationId, Name = "No Auth Office", OrganizationId = OrgId };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);
        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => cachedCustomerService.GetIdAsync(A<CancellationToken>._)).Returns(CustomerId);
        A.CallTo(() => organizationAuthorizationService.CanViewAnalyticsAsync(OrgId, CustomerId, A<CancellationToken>._))
            .Returns(false);

        var sut = new LocationAnalyticsService(
            repositoryFactory, locationService, cachedCustomerService, organizationAuthorizationService);

        var from = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 4, 7, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = await sut.GetAnalyticsAsync(LocationId, from, until, cancellationToken);

        // Assert – empty analytics returned for unauthorized access
        result.DesksOccupancyPercentage.ShouldBeEmpty();
        result.DailyBookingsTotal.ShouldBeEmpty();
        result.RoomsOccupancyPercentage.ShouldBeEmpty();
        result.ResourceAvailabilitySnapshots.ShouldBeEmpty();
    }
}
