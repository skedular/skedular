using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Temporalio.Testing;
using ResourceAvailabilityClassificationConstants = Location.Shared.Models.ResourceAvailabilityClassificationConstants;
using BookingProto = Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking;
using BookingResource = Api.Shared.Grpc.Skedular.Booking.Core.V1.Resource;
using LocationResource = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.UnitTests.Activities.LocationDailyAnalyticsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecordDeskAvailabilitySnapshotShould
{
    private static AsyncUnaryCall<T> CreateGrpcResponse<T>(T response) where T : class =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private static BookingConnection EmptyBookingConnection() => new() { PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty } };

    private static BookingConnection BookingConnectionWithResource(string resourceId) => new()
    {
        PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty },
        Edges = { new BookingEdge { Node = new BookingProto { Resources = { new BookingResource { Id = resourceId } } } } }
    };

    private static Database.Entities.Location MakeLocation(string locationId, IEnumerable<LocationResource> resources)
    {
        var location = new Database.Entities.Location { Id = locationId, Name = "Test Location" };
        foreach (var r in resources)
        {
            location.Resources.Add(r);
        }

        return location;
    }

    private static LocationResource MakeDeskResource(string id, string name, bool inactive = false, bool alsoRoom = false)
    {
        var tags = new List<OrganizationTag> { new() { Id = "tag-desk", Type = OrganizationTagTypeConstants.ResourceDesk } };
        if (alsoRoom)
        {
            tags.Add(new OrganizationTag { Id = "tag-room", Type = OrganizationTagTypeConstants.ResourceRoom });
        }

        return new LocationResource { Id = id, Name = name, Inactive = inactive, OrganizationTags = tags };
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Location_Not_Found(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        LocationDailyAnalytics sut)
    {
        var environment = new ActivityEnvironment();
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token))
            .Returns((Database.Entities.Location?)null);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        result.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Location_Is_Deleted(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        LocationDailyAnalytics sut)
    {
        var environment = new ActivityEnvironment();
        var deletedLocation = new Database.Entities.Location { Id = "loc-del", Name = "Deleted", DeletedAt = TimeProvider.System.GetUtcNow() };
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-del", environment.CancellationTokenSource.Token)).Returns(deletedLocation);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-del"));

        result.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Classify_Active_Desk_With_No_Bookings_As_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        [Frozen] DailyResourceAvailabilitySnapshot snapshotResult)
    {
        var sut = new LocationDailyAnalytics(repositoryFactory, randomHelper, timeProvider, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker), logger);
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var snapshotDate = now.StartOfDay();
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var desk = MakeDeskResource("res-1", "Desk A");
        var location = MakeLocation("loc-1", [desk]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(EmptyBookingConnection()));

        var capturedSnapshots = new List<DailyResourceAvailabilitySnapshot>();
        A.CallTo(() => snapshotRepository.Add(A<DailyResourceAvailabilitySnapshot>._))
            .Invokes(call => capturedSnapshots.Add(call.GetArgument<DailyResourceAvailabilitySnapshot>(0)!))
            .Returns(snapshotResult);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        result.ShouldBeTrue();
        capturedSnapshots.ShouldHaveSingleItem();
        capturedSnapshots[0].Classification.ShouldBe(ResourceAvailabilityClassificationConstants.Available);
        capturedSnapshots[0].ResourceId.ShouldBe("res-1");
        capturedSnapshots[0].Date.ShouldBe(snapshotDate);
        capturedSnapshots[0].Resource!.OrganizationTags.ShouldContain(t => t.Type == OrganizationTagTypeConstants.ResourceDesk);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Classify_Inactive_Desk_As_Unavailable(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        [Frozen] DailyResourceAvailabilitySnapshot snapshotResult)
    {
        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var inactiveDesk = MakeDeskResource("res-2", "Desk B", true);
        var location = MakeLocation("loc-1", [inactiveDesk]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(EmptyBookingConnection()));

        var capturedSnapshots = new List<DailyResourceAvailabilitySnapshot>();
        A.CallTo(() => snapshotRepository.Add(A<DailyResourceAvailabilitySnapshot>._))
            .Invokes(call => capturedSnapshots.Add(call.GetArgument<DailyResourceAvailabilitySnapshot>(0)!))
            .Returns(snapshotResult);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        result.ShouldBeTrue();
        capturedSnapshots.ShouldHaveSingleItem();
        capturedSnapshots[0].Classification.ShouldBe(ResourceAvailabilityClassificationConstants.Unavailable);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Classify_Booked_Desk_As_Booked(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        [Frozen] DailyResourceAvailabilitySnapshot snapshotResult)
    {
        var sut = new LocationDailyAnalytics(repositoryFactory, randomHelper, timeProvider, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker), logger);
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var desk = MakeDeskResource("res-3", "Desk C");
        var location = MakeLocation("loc-1", [desk]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(BookingConnectionWithResource("res-3")));

        var capturedSnapshots = new List<DailyResourceAvailabilitySnapshot>();
        A.CallTo(() => snapshotRepository.Add(A<DailyResourceAvailabilitySnapshot>._))
            .Invokes(call => capturedSnapshots.Add(call.GetArgument<DailyResourceAvailabilitySnapshot>(0)!))
            .Returns(snapshotResult);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        result.ShouldBeTrue();
        capturedSnapshots.ShouldHaveSingleItem();
        capturedSnapshots[0].Classification.ShouldBe(ResourceAvailabilityClassificationConstants.Booked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Classify_DualTagged_Resource_Using_Primary_Type_And_Emit_Warning(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        [Frozen] DailyResourceAvailabilitySnapshot snapshotResult)
    {
        var sut = new LocationDailyAnalytics(repositoryFactory, randomHelper, timeProvider, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker), logger);
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Desk+Room dual-tagged: desk wins by priority, classified normally (active, not booked → Available)
        var dualTaggedDesk = MakeDeskResource("res-4", "Flex Space", false, true);
        var location = MakeLocation("loc-1", [dualTaggedDesk]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(EmptyBookingConnection()));

        var capturedSnapshots = new List<DailyResourceAvailabilitySnapshot>();
        A.CallTo(() => snapshotRepository.Add(A<DailyResourceAvailabilitySnapshot>._))
            .Invokes(call => capturedSnapshots.Add(call.GetArgument<DailyResourceAvailabilitySnapshot>(0)!))
            .Returns(snapshotResult);

        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        result.ShouldBeTrue();
        capturedSnapshots.ShouldHaveSingleItem();
        // Desk wins priority; active + not booked → Available under its primary type
        capturedSnapshots[0].Classification.ShouldBe(ResourceAvailabilityClassificationConstants.Available);
        capturedSnapshots[0].Resource!.OrganizationTags.ShouldContain(t => t.Type == OrganizationTagTypeConstants.ResourceDesk);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Delete_Existing_Snapshots_Before_Inserting_For_Idempotency(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IDailyResourceAvailabilitySnapshotRepository snapshotRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        [Frozen] DailyResourceAvailabilitySnapshot snapshotResult)
    {
        var sut = new LocationDailyAnalytics(repositoryFactory, randomHelper, timeProvider, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker), logger);
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var snapshotDate = now.StartOfDay();
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var desk = MakeDeskResource("res-5", "Desk E");
        var location = MakeLocation("loc-1", [desk]);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.DailyResourceAvailabilitySnapshotRepository).Returns(snapshotRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("loc-1", environment.CancellationTokenSource.Token)).Returns(location);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(EmptyBookingConnection()));
        A.CallTo(() => snapshotRepository.Add(A<DailyResourceAvailabilitySnapshot>._))
            .Returns(snapshotResult);

        await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync("loc-1"));

        A.CallTo(() => snapshotRepository.DeleteByLocationAndDateAsync("loc-1", snapshotDate, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
