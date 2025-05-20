using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, bool includeDeletedResources, CancellationToken cancellationToken);
    Location Update(Location location);
    Location Remove(Location location);
    Task<ICollection<Location>> GetByCustomerIdAsync(string customerId, bool includeDeletedResources, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetByOrganizationIdAsync(string organizationId, bool includeDeletedResources, CancellationToken cancellationToken);
}

internal static class LocationExtensions
{
    internal static IIncludableQueryable<Location, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Location> originalQuery,
        bool includeDeletedResource,
        bool includeInactiveResource) =>
        originalQuery
            .Include(query => query.Resources.Where(resource =>
                includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))))
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Resources.Where(resource =>
                includeDeletedResource || (!resource.DeletedAt.HasValue && (includeInactiveResource || !resource.Inactive))))
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .Include(query => query.PreferredByCustomers)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));
}

public class LocationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

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

    public async Task<Location?> GetByIdAsync(string id, bool includeDeletedResources, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedResources, true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Location>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedResources,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue &&
                            query.Organization != null && !query.Organization.DeletedAt.HasValue &&
                            query.Organization.OrganizationMembers.Any(organizationMember =>
                                !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == customerId))
            .AddDependentObjects(includeDeletedResources, false)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Location>> GetByOrganizationIdAsync(
        string organizationId,
        bool includeDeletedResources,
        CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => !query.DeletedAt.HasValue && query.Organization != null && query.Organization.Id == organizationId)
            .AddDependentObjects(includeDeletedResources, false)
            .ToListAsync(cancellationToken);
}
