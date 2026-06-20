using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Grpc.Core;
using Location.Shared.Activities;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using NanoidDotNet;
using Temporalio.Testing;
using LocationResource = Location.Shared.Database.Entities.Resource;
using LocationEntity = Location.Shared.Database.Entities.Location;
using ResourceAvailabilityClassificationConstants = Location.Shared.Models.ResourceAvailabilityClassificationConstants;

namespace Location.Domain.IntegrationTests.Activities.LocationDailyAnalyticsTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class RecordDeskAvailabilitySnapshotShould(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
{
    private const int DeskCount = 22;

    /// <summary>Sets up a fake <see cref="CallInvoker" /> to return an empty booking connection for any gRPC call.</summary>
    private static void SetupEmptyBookingCallInvoker(CallInvoker callInvoker) =>
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetPaginatedBookingsInput, BookingConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetPaginatedBookingsInput>._))
            .Returns(new AsyncUnaryCall<BookingConnection>(
                Task.FromResult(new BookingConnection { PageInfo = new PageInfo { HasNextPage = false, EndCursor = string.Empty } }),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

    private async Task<string> SeedLocationWithDesksAsync(int deskCount, CancellationToken cancellationToken)
    {
        var organizationId = await Nanoid.GenerateAsync();
        var locationId = await Nanoid.GenerateAsync();
        var deskTagId = await Nanoid.GenerateAsync();
        var now = timeProvider.GetUtcNow();

        var org = new Organization { Id = organizationId, CreatedAt = now };
        await repositoryFactory.DbContext.Organization.AddAsync(org, cancellationToken);

        var location = new LocationEntity
        {
            Id = locationId,
            Name = "Integration Test Location",
            OrganizationId = organizationId,
            Type = LocationTypeConstants.Private,
            CreatedAt = now
        };
        await repositoryFactory.DbContext.Location.AddAsync(location, cancellationToken);

        var deskTag = new OrganizationTag { Id = deskTagId, Type = OrganizationTagTypeConstants.ResourceDesk, Organization = org, CreatedAt = now };
        await repositoryFactory.DbContext.OrganizationTag.AddAsync(deskTag, cancellationToken);

        for (var i = 0; i < deskCount; i++)
        {
            var resource = new LocationResource
            {
                Id = await Nanoid.GenerateAsync(), Name = $"Desk {i + 1:D2}", Location = location, CreatedAt = now
            };
            resource.OrganizationTags.Add(deskTag);
            await repositoryFactory.DbContext.Resource.AddAsync(resource, cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return locationId;
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_One_Snapshot_Per_Desk(
        [Frozen] CallInvoker callInvoker,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        CancellationToken cancellationToken)
    {
        SetupEmptyBookingCallInvoker(callInvoker);
        var locationId = await SeedLocationWithDesksAsync(DeskCount, cancellationToken);

        var bookingConfiguration = new BookingConfiguration { GrpcUrl = new Uri("http://localhost:5999"), ApiKey = "test-key" };
        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);

        var environment = new ActivityEnvironment();
        var result = await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync(locationId));

        result.ShouldBeTrue();

        var snapshotDate = timeProvider.GetUtcNow().StartOfDay();
        var snapshots = await repositoryFactory.DailyResourceAvailabilitySnapshotRepository
            .GetByLocationIdAndDateRangeAsync(locationId, snapshotDate, snapshotDate.AddDays(1), null, cancellationToken);

        snapshots.Count.ShouldBe(DeskCount);
        snapshots.ShouldAllBe(s => s.Classification == ResourceAvailabilityClassificationConstants.Available);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Replace_Existing_Snapshots_On_Second_Invocation(
        [Frozen] CallInvoker callInvoker,
        [Frozen] ILogger<LocationDailyAnalytics> logger,
        CancellationToken cancellationToken)
    {
        SetupEmptyBookingCallInvoker(callInvoker);
        var locationId = await SeedLocationWithDesksAsync(DeskCount, cancellationToken);

        var bookingConfiguration = new BookingConfiguration { GrpcUrl = new Uri("http://localhost:5999"), ApiKey = "test-key" };
        var sut = new LocationDailyAnalytics(
            repositoryFactory,
            randomHelper,
            timeProvider,
            bookingConfiguration,
            new BookingService.BookingServiceClient(callInvoker),
            logger);

        var environment = new ActivityEnvironment();

        // First invocation
        await environment.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync(locationId));

        // Second invocation — idempotent replace
        var environment2 = new ActivityEnvironment();
        var result2 = await environment2.RunAsync(() => sut.RecordResourceAvailabilitySnapshotAsync(locationId));

        result2.ShouldBeTrue();

        var snapshotDate = timeProvider.GetUtcNow().StartOfDay();
        var snapshots = await repositoryFactory.DailyResourceAvailabilitySnapshotRepository
            .GetByLocationIdAndDateRangeAsync(locationId, snapshotDate, snapshotDate.AddDays(1), null, cancellationToken);

        // Still exactly DeskCount records, not doubled
        snapshots.Count.ShouldBe(DeskCount);
    }
}
