using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, bool includeDeletedResources, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<ICollection<Location>> GetAllWithActiveOrganizationAsync(
        bool includeDeletedResources,
        bool includeInactiveResources,
        ICollection<string> productTagIds,
        CancellationToken cancellationToken);

    Location Update(Location location);
    Location Remove(Location location);
    Task<ICollection<Location>> GetByCustomerIdAsync(string customerId, bool includeDeletedResources, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetByOrganizationIdAsync(string organizationId, bool includeDeletedResources, CancellationToken cancellationToken);
}

internal static class LocationExtensions
{
    extension(IQueryable<Location> originalQuery)
    {
        internal IIncludableQueryable<Location, IEnumerable<OrganizationTag>> AddDependentObjects(
            bool includeDeletedResource,
            bool includeInactiveResource,
            ICollection<string> productTagIds) =>
            originalQuery
                .Include(query => query.Resources.Where(resource =>
                    (includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))) &&
                    (productTagIds.Count == 0 ||
                     resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id)))))
                .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.Resources.Where(resource =>
                    (includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))) &&
                    (productTagIds.Count == 0 ||
                     resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id)))))
                .Include(query => query.Organization)
                .ThenInclude(query => query!.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));
    }
}

public class LocationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

    public async Task<Location?> GetByIdAsync(string id, bool includeDeletedResources, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedResources, true, [])
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    /// <summary>
    ///     Returns the active locations for the supplied identifiers with only the organization relationship loaded.
    /// </summary>
    /// <param name="ids">The location identifiers to resolve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The non-deleted locations that match the supplied identifiers.</returns>
    /// <remarks>
    ///     This lightweight authorization lookup replaces the heavier specification path and intentionally loads only the organization data needed by
    ///     booking access checks.
    /// </remarks>
    public async Task<ICollection<Location>> GetActiveByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && ids.Contains(query.Id))
            .AsNoTrackingWithIdentityResolution()
            .Include(query => query.Organization)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Location>> GetAllWithActiveOrganizationAsync(
        bool includeDeletedResources,
        bool includeInactiveResources,
        ICollection<string> productTagIds,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && (query.Organization == null || !query.Organization.DeletedAt.HasValue))
            .Where(query =>
                productTagIds.Count == 0 ||
                query.Resources.Any(resource =>
                    (includeDeletedResources || (!resource.DeletedAt.HasValue && (includeInactiveResources || !resource.Inactive))) &&
                    resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && productTagIds.Contains(tag.Id))))
            .AddDependentObjects(includeDeletedResources, includeInactiveResources, productTagIds)
            .ToListAsync(cancellationToken);

    public Location Remove(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public async Task<ICollection<Location>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedResources,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Organization != null && !query.Organization.DeletedAt.HasValue &&
                            query.Organization.OrganizationMembers.Any(organizationMember =>
                                !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == customerId))
            .AddDependentObjects(includeDeletedResources, false, [])
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Location>> GetByOrganizationIdAsync(
        string organizationId,
        bool includeDeletedResources,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && query.Organization != null && query.Organization.Id == organizationId)
            .AddDependentObjects(includeDeletedResources, false, [])
            .ToListAsync(cancellationToken);
}
