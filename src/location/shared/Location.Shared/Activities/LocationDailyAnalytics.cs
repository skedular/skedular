using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Location.Shared.Database.Entities;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using DailyDeskCountRecording = Location.Shared.Database.Entities.DailyDeskCountRecording;
using DailyRoomCountRecording = Location.Shared.Database.Entities.DailyRoomCountRecording;
using ResourceAvailabilityClassification = Location.Shared.Models.ResourceAvailabilityClassification;
using Resource = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.Activities;

public class LocationDailyAnalytics(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    ILogger<LocationDailyAnalytics> logger)
{
    [Activity]
    public async Task<bool> RecordLocationDesksCountAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return false;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        var deskResources = location.Resources
            .Where(item =>
                item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceDesk) &&
                item.IsNotDeleted())
            .ToList();

        foreach (var resource in deskResources.Where(r =>
                     r.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceRoom)))
        {
            logger.LogWarning(
                "Resource has both desk and room tags, counting as desk only. ResourceId={ResourceId} LocationId={LocationId}",
                resource.Id, locationId);
        }

        _ = repositoryFactory.DailyDeskCountRecordingRepository.Add(new DailyDeskCountRecording
        {
            Id = randomHelper.Generate(), Count = deskResources.Count, Date = startOfToday, Location = location
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task<bool> RecordLocationRoomsCountAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return false;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        _ = repositoryFactory.DailyRoomCountRecordingRepository.Add(new DailyRoomCountRecording
        {
            Id = randomHelper.Generate(),
            Count = location.Resources
                .Count(item =>
                    item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceRoom) &&
                    !item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceDesk) &&
                    item.IsNotDeleted()),
            Date = startOfToday,
            Location = location
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task<bool> RecordResourceAvailabilitySnapshotAsync(string locationId)
        => await RecordResourceAvailabilitySnapshotForDateAsync(locationId, null);

    [Activity]
    public async Task<bool> RecordResourceAvailabilitySnapshotForDateAsync(string locationId, DateTimeOffset? snapshotDateOverride)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var workflowRunId = ActivityExecutionContext.Current.Info.WorkflowRunId;

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            logger.LogWarning(
                "Location not found or deleted, skipping snapshot. LocationId={LocationId} WorkflowRunId={WorkflowRunId}",
                locationId, workflowRunId);
            return false;
        }

        var snapshotDate = (snapshotDateOverride ?? timeProvider.GetUtcNow()).StartOfDay();
        var dayEnd = snapshotDate.AddDays(1);

        logger.LogInformation(
            "Starting resource availability snapshot. LocationId={LocationId} SnapshotDate={SnapshotDate} WorkflowRunId={WorkflowRunId}",
            locationId, snapshotDate, workflowRunId);

        // Fetch bookings scoped to today's UTC calendar day.
        // Cancelled bookings are already excluded server-side (BookingRepository filters !DeletedAt).
        var bookedResourceIds = await GetDayBookedResourceIdsAsync(locationId, snapshotDate, dayEnd, cancellationToken);

        // Determine primary resource type by priority: Desk > Room > Parking > Others.
        // Resources without any resource-type tag are excluded from the snapshot.
        static string? DetermineResourceType(Resource resource)
        {
            var tagTypes = resource.OrganizationTags.Select(t => t.Type).ToHashSet();
            if (tagTypes.Contains(OrganizationTagTypeConstants.ResourceDesk))
            {
                return OrganizationTagTypeConstants.ResourceDesk;
            }

            if (tagTypes.Contains(OrganizationTagTypeConstants.ResourceRoom))
            {
                return OrganizationTagTypeConstants.ResourceRoom;
            }

            if (tagTypes.Contains(OrganizationTagTypeConstants.ResourceParking))
            {
                return OrganizationTagTypeConstants.ResourceParking;
            }

            if (tagTypes.Contains(OrganizationTagTypeConstants.ResourceOthers))
            {
                return OrganizationTagTypeConstants.ResourceOthers;
            }

            return null;
        }

        var resourceTypeTagTypes = new HashSet<string>
        {
            OrganizationTagTypeConstants.ResourceDesk,
            OrganizationTagTypeConstants.ResourceRoom,
            OrganizationTagTypeConstants.ResourceParking,
            OrganizationTagTypeConstants.ResourceOthers
        };

        var typedResources = location.Resources
            .Where(r => r.IsNotDeleted())
            .Select(r => (Resource: r, PrimaryType: DetermineResourceType(r)))
            .Where(r => r.PrimaryType != null)
            .ToList();

        // Warn about resources carrying more than one resource-type tag (multi-typed resources).
        foreach (var (resource, primaryType) in typedResources)
        {
            var resourceTypeTagCount = resource.OrganizationTags.Count(item => item.Type is not null && resourceTypeTagTypes.Contains(item.Type));
            if (resourceTypeTagCount > 1)
            {
                logger.LogWarning(
                    "Resource has multiple resource type tags, using primary type. ResourceId={ResourceId} LocationId={LocationId} WorkflowRunId={WorkflowRunId} PrimaryType={PrimaryType}",
                    resource.Id, locationId, workflowRunId, primaryType);
            }
        }

        // Delete existing records for idempotent replace
        await repositoryFactory.DailyResourceAvailabilitySnapshotRepository
            .DeleteByLocationAndDateAsync(locationId, snapshotDate, cancellationToken);

        int available = 0, unavailable = 0, booked = 0;

        foreach (var (resource, primaryType) in typedResources)
        {
            ResourceAvailabilityClassification classification;
            if (resource.Inactive)
            {
                classification = ResourceAvailabilityClassification.Unavailable;
                unavailable++;
            }
            else if (bookedResourceIds.Contains(resource.Id))
            {
                classification = ResourceAvailabilityClassification.Booked;
                booked++;
            }
            else
            {
                classification = ResourceAvailabilityClassification.Available;
                available++;
            }

            _ = repositoryFactory.DailyResourceAvailabilitySnapshotRepository.Add(new DailyResourceAvailabilitySnapshot
            {
                Id = randomHelper.Generate(),
                LocationId = locationId,
                Location = location,
                ResourceId = resource.Id,
                Resource = resource,
                Date = snapshotDate,
                Classification = classification.ToResourceAvailabilityClassification()
            });
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Resource availability snapshot complete. LocationId={LocationId} WorkflowRunId={WorkflowRunId} Total={Total} Available={Available} Unavailable={Unavailable} Booked={Booked}",
            locationId, workflowRunId, typedResources.Count, available, unavailable, booked);

        return true;
    }

    private async Task<HashSet<string>> GetDayBookedResourceIdsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        // Cancelled bookings are already excluded server-side — see T008 finding in LocationBookingDerivedState.cs.
        var result = new HashSet<string>();
        string? after = null;

        do
        {
            var response = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
                new Admin_GetPaginatedBookingsInput
                {
                    After = after ?? string.Empty,
                    First = 1000,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new BookingWhereInput
                    {
                        LocationIds = { locationId },
                        FromGte = Timestamp.FromDateTimeOffset(from),
                        FromLt = Timestamp.FromDateTimeOffset(until)
                    },
                    OrderBy = { new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From } }
                },
                bookingConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);

            foreach (var edge in response.Edges)
            {
                foreach (var resource in edge.Node.Resources)
                {
                    result.Add(resource.Id);
                }
            }

            after = response.PageInfo.HasNextPage ? response.PageInfo.EndCursor : null;
        } while (!string.IsNullOrWhiteSpace(after));

        return result;
    }
}
