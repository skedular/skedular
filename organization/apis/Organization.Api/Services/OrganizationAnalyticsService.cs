using Api.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Organization.Api.GraphQL.Analytics;
using Organization.Api.Services.Authorization;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

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

        var dailyBookingCounts = await repositoryFactory.DbContext.DailyBookingCountRecording
            .Where(item =>
                !item.DeletedAt.HasValue && item.Organization.Id == organization.Id && item.Date >= from && item.Date <= until)
            .OrderBy(item => item.Date)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        var dailyMemberCounts = await repositoryFactory.DailyMemberCountRecordingRepository
            .GetByOrganizationIdAndDateRangeAsync(organization.Id, from, until, cancellationToken);

        var organizationMemberAttendancePercentages = dailyMemberCounts.Select(item =>
        {
            if (item.Count == 0)
            {
                return new OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = 0 };
            }

            var matchedBookingsCount = dailyBookingCounts
                .Where(recording => recording.Date == item.Date)
                .Select(recording => recording.Count)
                .SingleOrDefault();

            return new OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = matchedBookingsCount / (float)item.Count * 100 };
        }).ToList();

        var organizationDailyBookingsTotals = dailyBookingCounts
            .Select(item => new OrganizationDailyBookingsTotal { Date = item.Date, Total = item.Count })
            .ToList();

        return new OrganizationAnalytics
        {
            MemberAttendancePercentage = organizationMemberAttendancePercentages, DailyBookingsTotals = organizationDailyBookingsTotals
        };
    }
}
