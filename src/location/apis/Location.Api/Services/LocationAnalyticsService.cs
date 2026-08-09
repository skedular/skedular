using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Resource = Location.Shared.Database.Entities.Resource;

namespace Location.Api.Services;

public interface ILocationAnalyticsService
{
    Task<LocationAnalytics> GetAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LocationAnalytics>> GetAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class LocationAnalyticsService(
    IRepositoryFactory repositoryFactory,
    ILocationService locationService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService) : ILocationAnalyticsService
{
    public async Task<LocationAnalytics> GetAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();
        if (!await organizationAuthorizationService.CanViewAnalyticsAsync(location.OrganizationId, customerId, cancellationToken))
        {
            return new LocationAnalytics(locationId, string.Empty, [], [], [], []);
        }

        return (await GetAnalyticsAsync(
            [locationId],
            new Dictionary<string, string>
            {
                { location.Id, location.Name },
            },
            from,
            until,
            cancellationToken)).First();
    }

    public async Task<IReadOnlyList<LocationAnalytics>> GetAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationSearchCriteria searchCriteria,
        IReadOnlyList<LocationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is null)
        {
            return [];
        }

        var locations = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(null, null, null, null),
            searchCriteria,
            orderByFields,
            false,
            cancellationToken);

        if (locations.Item2.Count == 0)
        {
            return [];
        }

        var customerId = customer.Id;
        foreach (var item in locations.Item2)
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(item.Node.Id, cancellationToken) ?? throw new LocationNotFound();
            if (!await organizationAuthorizationService.CanViewAnalyticsAsync(location.OrganizationId, customerId, cancellationToken))
            {
                return [];
            }
        }

        return await GetAnalyticsAsync(
            [.. locations.Item2.Select(item => item.Node.Id)],
            locations.Item2.ToDictionary(item => item.Node.Id, item => item.Node.Name),
            from,
            until,
            cancellationToken);
    }

    private async Task<IReadOnlyList<LocationAnalytics>> GetAnalyticsAsync(
        List<string> locationIds,
        Dictionary<string, string> locationNames,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var dailyBookingCounts = await repositoryFactory.DailyBookingCountRecordingRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            cancellationToken);
        var dailyDeskBookingCounts = await repositoryFactory.DailyDeskBookingCountRecordingRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            cancellationToken);
        var dailyRoomBookingCounts = await repositoryFactory.DailyRoomBookingCountRecordingRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            cancellationToken);
        var dailyDeskCounts = await repositoryFactory.DailyDeskCountRecordingRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            cancellationToken);
        var dailyRoomCounts = await repositoryFactory.DailyRoomCountRecordingRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            cancellationToken);

        var allResourceAvailabilitySnapshots = await repositoryFactory.DailyResourceAvailabilitySnapshotRepository.GetByLocationIdsAndDateRangeAsync(
            locationIds,
            from,
            until,
            null,
            cancellationToken);

        return
        [
            .. locationIds.Select(locationId =>
            {
                var desksOccupancyPercentage = dailyDeskCounts
                    .Where(item => item.Location.Id == locationId && item.Count > 0)
                    .Select(item =>
                    {
                        var matchedBookingsCount = dailyDeskBookingCounts
                            .Where(recording => recording.Location.Id == locationId && recording.Date == item.Date)
                            .Select(recording => recording.Count)
                            .SingleOrDefault();

                        return new LocationDesksOccupancyPercentage
                        {
                            Date = item.Date,
                            Percentage = matchedBookingsCount / (float)item.Count * 100,
                        };
                    }).ToList();

                var dailyBookingsTotal = dailyBookingCounts
                    .Where(item => item.Location.Id == locationId)
                    .Select(item => new LocationDailyBookingsTotal
                    {
                        Date = item.Date,
                        Total = item.Count,
                    })
                    .ToList();

                var roomsOccupancyPercentage = dailyRoomCounts
                    .Where(item => item.Location.Id == locationId && item.Count > 0)
                    .Select(item =>
                    {
                        var matchedBookingsCount = dailyRoomBookingCounts
                            .Where(recording => recording.Location.Id == locationId && recording.Date == item.Date)
                            .Select(recording => recording.Count)
                            .SingleOrDefault();

                        return new LocationRoomsOccupancyPercentage
                        {
                            Date = item.Date,
                            Percentage = matchedBookingsCount / (float)item.Count * 100,
                        };
                    }).ToList();

                var resourceAvailabilitySnapshots = allResourceAvailabilitySnapshots
                    .Where(item => item.Location.Id == locationId)
                    .GroupBy(item => new
                    {
                        item.Date,
                        ResourceType = DetermineResourceType(item.Resource),
                    })
                    .Where(item => item.Key.ResourceType != null)
                    .Select(item => ResourceAvailabilitySnapshotReport.FromSnapshots(item.Key.Date, item.Key.ResourceType!, [.. item]))
                    .OrderBy(item => item.Date).ThenBy(r => r.ResourceType)
                    .ToList();

                return new LocationAnalytics(
                    locationId,
                    locationNames[locationId],
                    desksOccupancyPercentage,
                    dailyBookingsTotal,
                    roomsOccupancyPercentage,
                    resourceAvailabilitySnapshots);
            }),
        ];

        static string? DetermineResourceType(Resource resource)
        {
            var tagTypes = resource.OrganizationTags.Select(item => item.Type).ToHashSet();

            return tagTypes.Contains(OrganizationTagTypeConstants.ResourceDesk) ? OrganizationTagTypeConstants.ResourceDesk :
                tagTypes.Contains(OrganizationTagTypeConstants.ResourceRoom) ? OrganizationTagTypeConstants.ResourceRoom :
                tagTypes.Contains(OrganizationTagTypeConstants.ResourceParking) ? OrganizationTagTypeConstants.ResourceParking :
                tagTypes.Contains(OrganizationTagTypeConstants.ResourceOthers) ? OrganizationTagTypeConstants.ResourceOthers : null;
        }
    }
}
