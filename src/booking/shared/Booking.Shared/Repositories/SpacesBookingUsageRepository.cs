using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public sealed record SpacesBookingOrganizationUsage(string OrganizationId, DateTimeOffset BookingStartUtc);

public interface ISpacesBookingUsageRepository : IRepository<Organization>
{
    /// <summary>Returns the Organization with its Offering JSONB for a given org.</summary>
    Task<Organization?> GetOrganizationWithOfferingAsync(string organizationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Organization>> GetOrganizationsWithOfferingAsync(
        IReadOnlyCollection<string> organizationIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SpacesBookingOrganizationUsage>> GetBookingStartsByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        DateTimeOffset fromInclusive,
        DateTimeOffset toExclusive,
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

    public async Task<IReadOnlyList<SpacesBookingOrganizationUsage>> GetBookingStartsByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        DateTimeOffset fromInclusive,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken)
    {
        if (organizationIds.Count == 0)
        {
            return [];
        }

        return await DbContext.Booking
            .AsNoTracking()
            .Where(query =>
                !query.DeletedAt.HasValue &&
                query.From >= fromInclusive &&
                query.From < toExclusive &&
                query.InvolvedOrganizations.Any(organization =>
                    !organization.DeletedAt.HasValue && organizationIds.Contains(organization.Id)))
            .SelectMany(query => query.InvolvedOrganizations
                .Where(organization => !organization.DeletedAt.HasValue && organizationIds.Contains(organization.Id))
                .Select(organization => new SpacesBookingOrganizationUsage(organization.Id, query.From)))
            .ToListAsync(cancellationToken);
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
}
