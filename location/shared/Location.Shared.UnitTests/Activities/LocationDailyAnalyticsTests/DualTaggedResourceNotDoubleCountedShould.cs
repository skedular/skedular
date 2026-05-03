using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Temporalio.Testing;
using Testing.Shared.Assertions;
using LocationResource = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.UnitTests.Activities.LocationDailyAnalyticsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DualTaggedResourceNotDoubleCountedShould
{
    private static LocationResource MakeResource(string id, string name, bool hasDesk, bool hasRoom)
    {
        var tags = new List<OrganizationTag>();
        if (hasDesk)
        {
            tags.Add(new OrganizationTag { Id = $"tag-desk-{id}", Type = OrganizationTagTypeConstants.ResourceDesk });
        }

        if (hasRoom)
        {
            tags.Add(new OrganizationTag { Id = $"tag-room-{id}", Type = OrganizationTagTypeConstants.ResourceRoom });
        }

        return new LocationResource { Id = id, Name = name, OrganizationTags = tags };
    }

    private static Database.Entities.Location MakeLocation(string locationId, IEnumerable<LocationResource> resources)
    {
        var location = new Database.Entities.Location { Id = locationId, Name = "Test Location" };
        foreach (var item in resources)
        {
            location.Resources.Add(item);
        }

        return location;
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Dual_Tagged_Resource_Is_Counted_In_Desk_Count_Only(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyDeskCountRecordingRepository deskCountRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        IRandomHelper randomHelper,
        TimeProvider timeProvider,
        ILogger<LocationDailyAnalytics> logger)
    {
        // Arrange – one resource tagged as both desk AND room
        var environment = new ActivityEnvironment();
        var dualTaggedResource = MakeResource("res-dual", "Dual Desk", true, true);
        var location = MakeLocation("loc-1", [dualTaggedResource]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);

        var capturedRecordings = new List<DailyDeskCountRecording>();
        A.CallTo(() => deskCountRepository.Add(A<DailyDeskCountRecording>._))
            .Invokes(call => capturedRecordings.Add(call.GetArgument<DailyDeskCountRecording>(0)!))
            .Returns(A.Fake<DailyDeskCountRecording>());

        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);

        // Act
        var result = await environment.RunAsync(() => sut.RecordLocationDesksCountAsync("loc-1"));

        // Assert – resource is counted in desk total
        result.ShouldBeTrue();
        capturedRecordings.ShouldHaveSingleItem();
        capturedRecordings[0].Count.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Emits_Warning_Log_For_Dual_Tagged_Resource(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyDeskCountRecordingRepository deskCountRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        IRandomHelper randomHelper,
        TimeProvider timeProvider,
        ILogger<LocationDailyAnalytics> logger)
    {
        // Arrange
        var environment = new ActivityEnvironment();
        var dualTaggedResource = MakeResource("res-warn", "Warn Desk", true, true);
        var location = MakeLocation("loc-warn", [dualTaggedResource]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-warn", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => deskCountRepository.Add(A<DailyDeskCountRecording>._)).Returns(A.Fake<DailyDeskCountRecording>());

        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);

        // Act
        await environment.RunAsync(() => sut.RecordLocationDesksCountAsync("loc-warn"));

        // Assert – warning log contains the resource ID
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Pure_Desk_Resource_Is_Not_Warned(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyDeskCountRecordingRepository deskCountRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        IRandomHelper randomHelper,
        TimeProvider timeProvider,
        ILogger<LocationDailyAnalytics> logger)
    {
        // Arrange – resource has only desk tag, no room tag
        var environment = new ActivityEnvironment();
        var deskOnly = MakeResource("res-desk", "Pure Desk", true, false);
        var location = MakeLocation("loc-pure", [deskOnly]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyDeskCountRecordingRepository).Returns(deskCountRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-pure", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => deskCountRepository.Add(A<DailyDeskCountRecording>._)).Returns(A.Fake<DailyDeskCountRecording>());

        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);

        // Act
        await environment.RunAsync(() => sut.RecordLocationDesksCountAsync("loc-pure"));

        // Assert – no warning log for a resource that is only a desk
        LogAssertions.ACallToLog(logger, LogLevel.Warning).MustNotHaveHappened();
    }
}
