using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;

namespace Location.Api.Services;

public interface ILocationAnalyticsService
{
    Task<LocationAnalytics> GetAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);

    Task<ICollection<LocationAnalytics>> GetAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
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
            return new LocationAnalytics(locationId, string.Empty, [], [], []);
        }

        return (await GetAnalyticsAsync(
            [locationId],
            new Dictionary<string, string> { { location.Id, location.Name } },
            from,
            until,
            cancellationToken)).First();
    }

    public async Task<ICollection<LocationAnalytics>> GetAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationSearchCriteria searchCriteria,
        ICollection<LocationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
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

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        foreach (var item in locations.Item2)
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(item.Node.Id, cancellationToken) ?? throw new LocationNotFound();
            if (!await organizationAuthorizationService.CanViewAnalyticsAsync(location.OrganizationId, customerId, cancellationToken))
            {
                return [];
            }
        }

        return await GetAnalyticsAsync(
            locations.Item2.Select(item => item.Node.Id).ToList(),
            locations.Item2.ToDictionary(item => item.Node.Id, item => item.Node.Name),
            from,
            until,
            cancellationToken);
    }

    private async Task<ICollection<LocationAnalytics>> GetAnalyticsAsync(
        List<string> locationIds,
        Dictionary<string, string> locationNames,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var dailyBookingCounts = await repositoryFactory.DbContext.DailyBookingCountRecording
            .Where(item =>
                !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
        var dailyDeskBookingCounts = await repositoryFactory.DbContext.DailyDeskBookingCountRecording
            .Where(item =>
                !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);
        var dailyRoomBookingCounts = await repositoryFactory.DbContext.DailyRoomBookingCountRecording
            .Where(item =>
                !item.DeletedAt.HasValue && locationIds.Contains(item.Location.Id) && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .Include(item => item.Location)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

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

        return locationIds.Select(locationId =>
        {
            var desksOccupancyPercentage = dailyDeskCounts
                .Where(item => item.Location.Id == locationId)
                .Select(item =>
                {
                    if (item.Count == 0)
                    {
                        return new LocationDesksOccupancyPercentage { Date = item.Date, Percentage = 0 };
                    }

                    var matchedBookingsCount = dailyDeskBookingCounts
                        .Where(recording => recording.Location.Id == locationId && recording.Date == item.Date)
                        .Select(recording => recording.Count)
                        .SingleOrDefault();

                    return new LocationDesksOccupancyPercentage { Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100 };
                }).ToList();

            var dailyBookingsTotal = dailyBookingCounts
                .Where(item => item.Location.Id == locationId)
                .Select(item =>
                    new LocationDailyBookingsTotal { Date = item.Date, Total = item.Count })
                .ToList();

            var roomsOccupancyPercentage = dailyRoomCounts
                .Where(item => item.Location.Id == locationId)
                .Select(item =>
                {
                    if (item.Count == 0)
                    {
                        return new LocationRoomsOccupancyPercentage { Date = item.Date, Percentage = 0 };
                    }

                    var matchedBookingsCount = dailyRoomBookingCounts
                        .Where(recording => recording.Location.Id == locationId && recording.Date == item.Date)
                        .Select(recording => recording.Count)
                        .SingleOrDefault();

                    return new LocationRoomsOccupancyPercentage { Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100 };
                }).ToList();

            return new LocationAnalytics(
                locationId,
                locationNames[locationId],
                desksOccupancyPercentage,
                dailyBookingsTotal,
                roomsOccupancyPercentage);
        }).ToList();
    }
}
