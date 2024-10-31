using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Booking = Location.Shared.Database.Entities.Booking;
using DailyDeskCountRecording = Location.Shared.Database.Entities.DailyDeskCountRecording;

namespace Location.Api.Services;

public interface ILocationAnalyticsService
{
    Task<(ICollection<LocationDesksOccupancyPercentage>, ICollection<LocationDailyBookingsTotal>)>
        GetAnalyticsAsync(
            string locationId,
            DateTimeOffset from,
            DateTimeOffset until,
            CancellationToken cancellationToken);
}

public class LocationAnalyticsService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ILocationAuthorizationService locationAuthorizationService) : ILocationAnalyticsService
{
    public async Task<(ICollection<LocationDesksOccupancyPercentage>, ICollection<LocationDailyBookingsTotal>
            )>
        GetAnalyticsAsync(
            string locationId,
            DateTimeOffset from,
            DateTimeOffset until,
            CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanViewAnalytics(location, customer))
        {
            return ([], []);
        }

        var bookings = await repositoryFactory.BookingRepository.Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Location.Id == locationId && query.From >= from &&
                    query.To <= until.AddDays(1)
            }.AddInclude(query => query.Desks))
            .AsNoTracking().ToListAsync(cancellationToken);

        var dailyDeskCounts = await repositoryFactory.DailyDeskCountRecordingRepository
            .Query(new Specification<DailyDeskCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Location.Id == locationId && query.Date >= from &&
                        query.Date <= until
                }
                .ApplyOrderBy(query => query.Date))
            .AsNoTracking().ToListAsync(cancellationToken);

        var locationMemberAttendancePercentages = dailyDeskCounts.Select(item =>
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
                    booking.Desks.Count > 0);

            return new LocationDesksOccupancyPercentage
            {
                Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100
            };
        }).ToList();

        var locationDailyBookingsTotals = dailyDeskCounts.Select(item =>
        {
            var matchedBookingsCount = bookings.Count(
                booking =>
                    item.Date.Year == booking.From.Year &&
                    item.Date.Month == booking.From.Month &&
                    item.Date.Day == booking.From.Day);

            return new LocationDailyBookingsTotal { Date = item.Date, Total = matchedBookingsCount };
        }).ToList();

        return (locationMemberAttendancePercentages, locationDailyBookingsTotals);
    }
}
