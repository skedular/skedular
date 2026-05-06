using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Temporalio.Testing;
using BookingProto = Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking;
using LocationEntity = Location.Shared.Database.Entities.Location;
using LocationResource = Location.Shared.Database.Entities.Resource;
using Resource = Api.Shared.Grpc.Skedular.Booking.Core.V1.Resource;

namespace Location.Shared.UnitTests.Activities.LocationBookingDerivedStateTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecomputeIdempotencyShould
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

    [Theory]
    [AutoFakeItEasyData]
    public async Task Second_Invocation_Produces_Identical_Daily_Recordings(
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
        const string LocationId = "loc-idempotent";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Idempotent Office", OrganizationId = "org-idempotent", Type = LocationTypeConstants.Private
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

        var day = new DateTimeOffset(2026, 4, 15, 10, 0, 0, TimeSpan.Zero);
        var bookingResponse = new BookingConnection
        {
            PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty },
            Edges =
            {
                new BookingEdge { Node = new BookingProto { From = Timestamp.FromDateTimeOffset(day) } },
                new BookingEdge { Node = new BookingProto { From = Timestamp.FromDateTimeOffset(day) } }
            }
        };

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

        // Act – first invocation
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));
        var afterFirst = await dbContext.DailyBookingCountRecording.Include(r => r.Location).ToListAsync(TestContext.Current.CancellationToken);

        // Reset gRPC call counter to allow the second invocation to re-use the same mock
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(bookingResponse));

        // Act – second invocation with identical input
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));
        var afterSecond = await dbContext.DailyBookingCountRecording.Include(r => r.Location).ToListAsync(TestContext.Current.CancellationToken);

        // Assert – exactly 1 record per day after both invocations (no duplication)
        afterFirst.Count.ShouldBe(1);
        afterSecond.Count.ShouldBe(1);
        afterSecond[0].Count.ShouldBe(afterFirst[0].Count);
        afterSecond[0].Date.ShouldBe(afterFirst[0].Date);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Desk_Booking_Count_Is_Not_Doubled_On_Second_Invocation(
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
        const string LocationId = "loc-desk-idem";
        var deskResourceId = "res-desk";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Desk Office", OrganizationId = "org-desk", Type = LocationTypeConstants.Private
        };
        var deskResource = new LocationResource
        {
            Id = deskResourceId,
            Name = "Desk 1",
            OrganizationTags = [new OrganizationTag { Id = "tag-desk", Type = OrganizationTagTypeConstants.ResourceDesk }]
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
            .Returns(new[] { deskResource });

        var day = new DateTimeOffset(2026, 4, 15, 10, 0, 0, TimeSpan.Zero);
        var bookingResponse = new BookingConnection
        {
            PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty },
            Edges =
            {
                new BookingEdge
                {
                    Node = new BookingProto
                    {
                        From = Timestamp.FromDateTimeOffset(day), Resources = { new Resource { Id = deskResourceId } }
                    }
                }
            }
        };

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

        // Act – run twice
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(bookingResponse));
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert – desk booking count is 1, not 2
        var deskRecordings = await dbContext.DailyDeskBookingCountRecording.Include(r => r.Location)
            .ToListAsync(TestContext.Current.CancellationToken);
        deskRecordings.Count.ShouldBe(1);
        deskRecordings[0].Count.ShouldBe(1);
    }
}
