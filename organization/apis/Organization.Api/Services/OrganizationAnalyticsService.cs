using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Booking = Organization.Shared.Database.Entities.Booking;
using DailyMemberCountRecording = Organization.Shared.Database.Entities.DailyMemberCountRecording;

namespace Organization.Api.Services;

public interface IOrganizationAnalyticsService
{
    Task<OrganizationAnalytics> GetAnalyticsAsync(
        string? id,
        string? customDomain,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class OrganizationAnalyticsService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IOrganizationAnalyticsService
{
    public async Task<OrganizationAnalytics> GetAnalyticsAsync(
        string? id,
        string? customDomain,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(id, customDomain, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanViewAnalyticsAsync(organization, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var bookings = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.InvolvedOrganizations.Select(item => item.Id).Contains(organization.Id) &&
                    query.From >= from &&
                    query.Until <= until.AddDays(1)
            }).AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        var dailyMemberCounts = await repositoryFactory.DailyMemberCountRecordingRepository
            .Query(new Specification<DailyMemberCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Organization.Id == organization.Id && query.Date >= from &&
                        query.Date <= until
                }
                .ApplyOrderBy(query => query.Date))
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        var organizationMemberAttendancePercentages = dailyMemberCounts.Select(item =>
        {
            if (item.Count == 0)
            {
                return new OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = 0 };
            }

            var matchedBookingsCount = bookings.Count(booking =>
                item.Date.Year == booking.From.Year &&
                item.Date.Month == booking.From.Month &&
                item.Date.Day == booking.From.Day);

            return new OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100 };
        }).ToList();

        var organizationDailyBookingsTotals = dailyMemberCounts.Select(item =>
        {
            var matchedBookingsCount = bookings.Count(booking =>
                item.Date.Year == booking.From.Year &&
                item.Date.Month == booking.From.Month &&
                item.Date.Day == booking.From.Day);

            return new OrganizationDailyBookingsTotal { Date = item.Date, Total = matchedBookingsCount };
        }).ToList();

        return new OrganizationAnalytics(organization.Id, organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }
}
