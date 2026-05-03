using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Temporalio.Testing;
using BookingProto = Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking;
using LocationEntity = Location.Shared.Database.Entities.Location;
using LocationResource = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.UnitTests.Activities.LocationBookingDerivedStateTests;

// T008 finding: BookingRepository.GetPaginatedBookingsUntrackedAsync already applies
// .Where(item => !item.DeletedAt.HasValue) server-side, so cancelled bookings never
// reach the client. These tests verify that bookings returned by the server are correctly
// recorded per day. The "excluded" guarantee is enforced by the server contract, not
// by client-side filtering.
[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelledBookingsExcludedShould
{
    private static LocationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LocationDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString())
            .Options;
        return new TestLocationDbContext(options, new CustomDbContextOptions<LocationDbContext> { IsPooled = false });
    }

    private static AsyncUnaryCall<T> CreateGrpcResponse<T>(T response) where T : class =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private static BookingConnection BookingConnectionWith(params BookingEdge[] edges)
    {
        var conn = new BookingConnection { PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty } };
        conn.Edges.AddRange(edges);
        return conn;
    }

    private static BookingEdge MakeEdge(DateTimeOffset from) => new() { Node = new BookingProto { From = Timestamp.FromDateTimeOffset(from) } };

    [Theory]
    [AutoFakeItEasyData]
    public async Task Records_Correct_Daily_Count_For_Server_Returned_Bookings(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        IRandomHelper randomHelper,
        ICachedLocationService cachedLocationService)
    {
        // Arrange
        var environment = new ActivityEnvironment();
        const string LocationId = "loc-test";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Test Office", OrganizationId = "org-test", Type = LocationTypeConstants.Private
        };

        await using var dbContext = CreateInMemoryContext();

        var counter = 0;
        A.CallTo(() => randomHelper.Generate()).ReturnsLazily(() => $"id-{++counter}");
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(dbContext);
        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => locationRepository.Update(A<LocationEntity>._)).Returns(location);
        A.CallTo(() => resourceRepository.GetByIdsWithOrganizationTagsUntrackedAsync(A<IReadOnlyList<string>>._, A<CancellationToken>._))
            .Returns(Array.Empty<LocationResource>());

        var day1 = new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero);
        var day2 = new DateTimeOffset(2026, 4, 2, 9, 0, 0, TimeSpan.Zero);

        // Server returns 2 bookings on day1, 1 booking on day2 (server already excluded cancelled ones)
        var bookingResponse = BookingConnectionWith(
            MakeEdge(day1), MakeEdge(day1), MakeEdge(day2));

        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(bookingResponse));

        var sut = new LocationBookingDerivedState(
            repositoryFactory, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            randomHelper, cachedLocationService);

        // Act
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert – only 2 day records, matching the 2 distinct booking days the server returned
        var recordings = await dbContext.DailyBookingCountRecording.Include(r => r.Location).ToListAsync(TestContext.Current.CancellationToken);
        recordings.Count.ShouldBe(2);
        recordings.ShouldContain(r => r.Date == day1.StartOfDay() && r.Count == 2);
        recordings.ShouldContain(r => r.Date == day2.StartOfDay() && r.Count == 1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Records_Zero_Bookings_When_Server_Returns_None(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] BookingConfiguration bookingConfiguration,
        CallInvoker callInvoker,
        IRandomHelper randomHelper,
        ICachedLocationService cachedLocationService)
    {
        // Arrange – server returns empty list (all bookings were cancelled / none exist)
        var environment = new ActivityEnvironment();
        const string LocationId = "loc-empty";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Empty Office", OrganizationId = "org-empty", Type = LocationTypeConstants.Private
        };

        await using var dbContext = CreateInMemoryContext();

        var counter = 0;
        A.CallTo(() => randomHelper.Generate()).ReturnsLazily(() => $"id-{++counter}");
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(dbContext);
        A.CallTo(() => locationRepository.GetByIdAsync(LocationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => locationRepository.Update(A<LocationEntity>._)).Returns(location);
        A.CallTo(() => resourceRepository.GetByIdsWithOrganizationTagsUntrackedAsync(
                A<IReadOnlyList<string>>._, A<CancellationToken>._))
            .Returns(Array.Empty<LocationResource>());

        var emptyResponse = new BookingConnection { PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty } };

        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(emptyResponse));

        var sut = new LocationBookingDerivedState(
            repositoryFactory, bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            randomHelper, cachedLocationService);

        // Act
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert
        var recordings = await dbContext.DailyBookingCountRecording.ToListAsync(TestContext.Current.CancellationToken);
        recordings.ShouldBeEmpty();
    }
}
