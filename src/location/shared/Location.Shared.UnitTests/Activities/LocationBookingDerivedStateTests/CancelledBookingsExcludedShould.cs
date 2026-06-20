using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
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

namespace Location.Shared.UnitTests.Activities.LocationBookingDerivedStateTests;

// T008 finding: BookingRepository.GetPaginatedBookingsUntrackedAsync already applies
// .Where(item => !item.DeletedAt.HasValue) server-side, so cancelled bookings never
// reach the client. These tests verify that bookings returned by the server are correctly
// recorded per day. The "excluded" guarantee is enforced by the server contract, not
// by client-side filtering.
[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelledBookingsExcludedShould
{
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
        const string LocationId = "loc-test";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Test Office", OrganizationId = "org-test", Type = LocationTypeConstants.Private
        };

        var counter = 0;
        IReadOnlyList<DailyBookingCountRecording> recordings = [];
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
            .Invokes(call => recordings = call.GetArgument<IReadOnlyList<DailyBookingCountRecording>>(1)!.ToList())
            .Returns(Task.CompletedTask);
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

        // Act
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert – only 2 day records, matching the 2 distinct booking days the server returned
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
        // Arrange – server returns empty list (all bookings were cancelled / none exist)
        var environment = new ActivityEnvironment();
        const string LocationId = "loc-empty";
        var location = new LocationEntity
        {
            Id = LocationId, Name = "Empty Office", OrganizationId = "org-empty", Type = LocationTypeConstants.Private
        };

        var counter = 0;
        IReadOnlyList<DailyBookingCountRecording> recordings = [];
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
            .Invokes(call => recordings = call.GetArgument<IReadOnlyList<DailyBookingCountRecording>>(1)!.ToList())
            .Returns(Task.CompletedTask);
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

        // Act
        await environment.RunAsync(() => sut.RecomputeAsync(LocationId));

        // Assert
        recordings.ShouldBeEmpty();
    }
}
