using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public sealed record SpacesBookingUsageWindow(string OrganizationId, DateTimeOffset StartInclusive, DateTimeOffset EndExclusive);

public sealed record SpacesBookingOrganizationUsageCount(string OrganizationId, int Count);

public interface ISpacesBookingUsageRepository : IRepository<Organization>
{
    /// <summary>Returns the Organization with its Offering JSONB for a given org.</summary>
    Task<Organization?> GetOrganizationWithOfferingAsync(string organizationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Organization>> GetOrganizationsWithOfferingAsync(
        IReadOnlyCollection<string> organizationIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SpacesBookingOrganizationUsageCount>> CountCurrentPeriodBookingInstancesAsync(
        IReadOnlyCollection<SpacesBookingUsageWindow> windows,
        CancellationToken cancellationToken);

    Task<int> CountCurrentPeriodBookingInstancesAsync(
        string organizationId,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        CancellationToken cancellationToken);
}

public class SpacesBookingUsageRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Organization>(dbContext, timeProvider), ISpacesBookingUsageRepository
{
    private const int MaximumAggregateWindowCount = 100;

    public async Task<Organization?> GetOrganizationWithOfferingAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AsNoTracking()
            .Where(query => query.Id == organizationId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Organization>> GetOrganizationsWithOfferingAsync(
        IReadOnlyCollection<string> organizationIds,
        CancellationToken cancellationToken)
    {
        if (organizationIds.Count == 0)
        {
            return [];
        }

        return await DbContext.Organization
            .AsNoTracking()
            .Where(query => organizationIds.Contains(query.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpacesBookingOrganizationUsageCount>> CountCurrentPeriodBookingInstancesAsync(
        IReadOnlyCollection<SpacesBookingUsageWindow> windows,
        CancellationToken cancellationToken)
    {
        var distinctWindows = windows.DistinctBy(window => window.OrganizationId).ToList();
        if (distinctWindows.Count == 0)
        {
            return [];
        }

        var result = new List<SpacesBookingOrganizationUsageCount>(distinctWindows.Count);
        foreach (var chunk in distinctWindows.Chunk(MaximumAggregateWindowCount))
        {
            result.AddRange(await CountCurrentPeriodBookingInstancesAsync(chunk, cancellationToken));
        }

        return result;
    }

    public async Task<int> CountCurrentPeriodBookingInstancesAsync(
        string organizationId,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        CancellationToken cancellationToken)
        => await DbContext.Booking
            .AsNoTracking()
            .Where(query =>
                !query.DeletedAt.HasValue &&
                query.From >= periodStart &&
                query.From < periodEnd &&
                query.InvolvedOrganizations.Any(organization => !organization.DeletedAt.HasValue && organization.Id == organizationId))
            .CountAsync(cancellationToken);

    private async Task<IReadOnlyList<SpacesBookingOrganizationUsageCount>> CountCurrentPeriodBookingInstancesAsync(
        IReadOnlyList<SpacesBookingUsageWindow> distinctWindows,
        CancellationToken cancellationToken)
    {
        var firstWindow = distinctWindows[0];
        var firstOrganizationId = firstWindow.OrganizationId;
        var firstStartInclusive = firstWindow.StartInclusive;
        var firstEndExclusive = firstWindow.EndExclusive;
        var rows = DbContext.Booking
            .AsNoTracking()
            .Where(booking =>
                !booking.DeletedAt.HasValue &&
                booking.From >= firstStartInclusive &&
                booking.From < firstEndExclusive &&
                booking.InvolvedOrganizations.Any(organization =>
                    !organization.DeletedAt.HasValue && organization.Id == firstOrganizationId))
            .Select(booking => new
            {
                OrganizationId = firstOrganizationId,
                booking.Id,
            });

        foreach (var window in distinctWindows.Skip(1))
        {
            var organizationId = window.OrganizationId;
            var startInclusive = window.StartInclusive;
            var endExclusive = window.EndExclusive;
            rows = rows.Concat(DbContext.Booking
                .AsNoTracking()
                .Where(booking =>
                    !booking.DeletedAt.HasValue &&
                    booking.From >= startInclusive &&
                    booking.From < endExclusive &&
                    booking.InvolvedOrganizations.Any(organization =>
                        !organization.DeletedAt.HasValue && organization.Id == organizationId))
                .Select(booking => new
                {
                    OrganizationId = organizationId,
                    booking.Id,
                }));
        }

        return await rows
            .GroupBy(row => row.OrganizationId)
            .Select(group => new SpacesBookingOrganizationUsageCount(group.Key, group.Count()))
            .ToListAsync(cancellationToken);
    }
}
