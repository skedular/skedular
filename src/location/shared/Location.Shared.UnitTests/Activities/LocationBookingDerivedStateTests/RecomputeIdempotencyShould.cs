using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Temporalio.Testing;
using BookingProto = Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking;
using LocationEntity = Location.Shared.Database.Entities.Location;
using LocationResource = Location.Shared.Database.Entities.Resource;
using Resource = Api.Shared.Grpc.Skedular.Booking.Core.V1.Resource;

namespace Location.Shared.UnitTests.Activities.LocationBookingDerivedStateTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecomputeIdempotencyShould
{
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
        [Frozen] ILocationBookingRecordingRepository locationBookingRecordingRepository,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedLocationService cachedLocationService)
    {
        var sut = new LocationBookingDerivedState(
            repositoryFactory,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            randomHelper,
            cachedLocationService);
        // Arrange
        var environment = new ActivityEnvironment();
        const string LocationId = "loc-idempotent";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Idempotent Office", OrganizationId = "org-idempotent", Type = LocationTypeConstants.Private
        };

        var counter = 0;
        var capturedRecordings = new List<IReadOnlyList<DailyBookingCountRecording>>();
        A.CallTo(() => randomHelper.Generate()).ReturnsLazily(() => $"id-{++counter}");
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.LocationBookingRecordingRepository).Returns(locationBookingRecordingRepository);
        A.CallTo(() => locationBookingRecordingRepository.ReplaceDailyRecordingsAsync(
                LocationId,
                A<IReadOnlyList<DailyBookingCountRecording>>._,
                A<IReadOnlyList<DailyDeskBookingCountRecording>>._,
                A<IReadOnlyList<DailyRoomBookingCountRecording>>._,
                A<CancellationToken>._))
            .Invokes(call => capturedRecordings.Add(call.GetArgument<IReadOnlyList<DailyBookingCountRecording>>(1)!.ToList()))
            .Returns(Task.CompletedTask);
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

        // Act – first invocation
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Reset gRPC call counter to allow the second invocation to re-use the same mock
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(CreateGrpcResponse(bookingResponse));

        // Act – second invocation with identical input
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert – exactly 1 record per day after both invocations (no duplication)
        capturedRecordings.Count.ShouldBe(2);
        var afterFirst = capturedRecordings[0];
        var afterSecond = capturedRecordings[1];
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
        [Frozen] ILocationBookingRecordingRepository locationBookingRecordingRepository,
        [Frozen] BookingConfiguration bookingConfiguration,
        [Frozen] CallInvoker callInvoker,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedLocationService cachedLocationService)
    {
        var sut = new LocationBookingDerivedState(
            repositoryFactory,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            randomHelper,
            cachedLocationService);
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

        var counter = 0;
        IReadOnlyList<DailyDeskBookingCountRecording> deskRecordings = [];
        A.CallTo(() => randomHelper.Generate()).ReturnsLazily(() => $"id-{++counter}");
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.LocationBookingRecordingRepository).Returns(locationBookingRecordingRepository);
        A.CallTo(() => locationBookingRecordingRepository.ReplaceDailyRecordingsAsync(
                LocationId,
                A<IReadOnlyList<DailyBookingCountRecording>>._,
                A<IReadOnlyList<DailyDeskBookingCountRecording>>._,
                A<IReadOnlyList<DailyRoomBookingCountRecording>>._,
                A<CancellationToken>._))
            .Invokes(call => deskRecordings = call.GetArgument<IReadOnlyList<DailyDeskBookingCountRecording>>(2)!.ToList())
            .Returns(Task.CompletedTask);
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
        deskRecordings.Count.ShouldBe(1);
        deskRecordings[0].Count.ShouldBe(1);
    }
}
