using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Pagination;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Booking = Location.Shared.Database.Entities.Booking;
using DailyDeskCountRecording = Location.Shared.Database.Entities.DailyDeskCountRecording;
using DailyRoomCountRecording = Location.Shared.Database.Entities.DailyRoomCountRecording;

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
    ILocationAuthorizationService locationAuthorizationService) : ILocationAnalyticsService
{
    public async Task<LocationAnalytics> GetAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanViewAnalytics(location, customer))
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

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        foreach (var item in locations.Item2)
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(item.Node.Id, cancellationToken);
            if (location is null)
            {
                throw new LocationNotFound();
            }

            if (!locationAuthorizationService.CanViewAnalytics(location, customer))
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
        ICollection<string> locationIds,
        IDictionary<string, string> locationNames,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.Query(
                new Specification<Booking>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && locationIds.Contains(query.Location.Id) && query.From >= from && query.Until <= until.AddDays(1)
                    }
                    .AddInclude(query => query.Resources)
                    .AddInclude(query => query.Location))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dailyDeskCounts = await repositoryFactory.DailyDeskCountRecordingRepository
            .Query(new Specification<DailyDeskCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && locationIds.Contains(query.Location.Id) && query.Date >= from && query.Date <= until
                }
                .ApplyOrderBy(query => query.Date)
                .AddInclude(query => query.Location))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dailyRoomCounts = await repositoryFactory.DailyRoomCountRecordingRepository
            .Query(new Specification<DailyRoomCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && locationIds.Contains(query.Location.Id) && query.Date >= from && query.Date <= until
                }
                .ApplyOrderBy(query => query.Date)
                .AddInclude(query => query.Location))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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

                    var matchedBookingsCount = bookings.Count(
                        booking =>
                            item.Date.Year == booking.From.Year &&
                            item.Date.Month == booking.From.Month &&
                            item.Date.Day == booking.From.Day &&
                            booking.Resources.Count(resource => resource.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.Desk)) >
                            0);

                    return new LocationDesksOccupancyPercentage { Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100 };
                }).ToList();

            var dailyBookingsTotal = dailyDeskCounts
                .Where(item => item.Location.Id == locationId)
                .Select(item =>
                {
                    var matchedBookingsCount = bookings.Count(
                        booking =>
                            item.Date.Year == booking.From.Year &&
                            item.Date.Month == booking.From.Month &&
                            item.Date.Day == booking.From.Day);

                    return new LocationDailyBookingsTotal { Date = item.Date, Total = matchedBookingsCount };
                }).ToList();

            var roomsOccupancyPercentage = dailyRoomCounts
                .Where(item => item.Location.Id == locationId)
                .Select(item =>
                {
                    if (item.Count == 0)
                    {
                        return new LocationRoomsOccupancyPercentage { Date = item.Date, Percentage = 0 };
                    }

                    var matchedBookingsCount = bookings.Count(
                        booking =>
                            item.Date.Year == booking.From.Year &&
                            item.Date.Month == booking.From.Month &&
                            item.Date.Day == booking.From.Day &&
                            booking.Resources.Count(resource => resource.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.Room)) >
                            0);

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
