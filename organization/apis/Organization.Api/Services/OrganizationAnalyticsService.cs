using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Repositories;
using Booking = Organization.Shared.Database.Entities.Booking;
using DailyMemberCountRecording = Organization.Shared.Database.Entities.DailyMemberCountRecording;

namespace Organization.Api.Services;

public interface IOrganizationAnalyticsService
{
    Task<(ICollection<OrganizationMemberAttendancePercentage>, ICollection<OrganizationDailyBookingsTotal>)>
        GetAnalyticsAsync(
            string organizationId,
            DateTimeOffset from,
            DateTimeOffset until,
            CancellationToken cancellationToken);
}

public class OrganizationAnalyticsService(
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IOrganizationAnalyticsService
{
    public async Task<(ICollection<OrganizationMemberAttendancePercentage>, ICollection<OrganizationDailyBookingsTotal>
            )>
        GetAnalyticsAsync(
            string organizationId,
            DateTimeOffset from,
            DateTimeOffset until,
            CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewAnalytics(organization, customer))
        {
            return ([], []);
        }

        var bookings = await repositoryFactory.BookingRepository.Query(new Specification<Booking>
        {
            Criteria = query =>
                !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.From >= from &&
                query.To <= until.AddDays(1)
        }).AsNoTracking().ToListAsync(cancellationToken);

        var dailyMemberCounts = await repositoryFactory.DailyMemberCountRecordingRepository
            .Query(new Specification<DailyMemberCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.Date >= from &&
                        query.Date <= until
                }
                .ApplyOrderBy(query => query.Date))
            .AsNoTracking().ToListAsync(cancellationToken);

        var organizationMemberAttendancePercentages = dailyMemberCounts.Select(item =>
        {
            if (item.Count == 0)
            {
                return new OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = 0 };
            }

            var matchedBookingsCount = bookings.Count(
                booking =>
                    item.Date.Year == booking.From.Year &&
                    item.Date.Month == booking.From.Month &&
                    item.Date.Day == booking.From.Day);

            return new OrganizationMemberAttendancePercentage
            {
                Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100
            };
        }).ToList();

        var organizationDailyBookingsTotals = dailyMemberCounts.Select(item =>
        {
            var matchedBookingsCount = bookings.Count(
                booking =>
                    item.Date.Year == booking.From.Year &&
                    item.Date.Month == booking.From.Month &&
                    item.Date.Day == booking.From.Day);

            return new OrganizationDailyBookingsTotal { Date = item.Date, Total = matchedBookingsCount };
        }).ToList();

        return (organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }
}
