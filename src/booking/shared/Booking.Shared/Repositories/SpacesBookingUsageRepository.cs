using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface ISpacesBookingUsageRepository : IRepository<Organization>
{
    /// <summary>Returns the Organization with its Offering JSONB for a given org.</summary>
    Task<Organization?> GetOrganizationWithOfferingAsync(string organizationId, CancellationToken cancellationToken);

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
