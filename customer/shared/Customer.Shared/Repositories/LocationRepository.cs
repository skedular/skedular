using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Customer.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<ICollection<Location>> GetAllAsync(bool includeDeletedLocationMembers, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, bool includeDeletedLocationMembers, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

internal static class LocationExtensions
{
    internal static IIncludableQueryable<Location, ICollection<Database.Entities.Customer>> AddDependentObjects(
        this IQueryable<Location> originalQuery,
        bool includeDeletedLocationMembers) =>
        originalQuery
            .Include(query => query.LocationMembers.Where(locationMember => includeDeletedLocationMembers || !locationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Resources)
            .ThenInclude(query => query.OrganizationResourceType)
            .Include(query => query.Desks)
            .Include(query => query.Rooms)
            .Include(query => query.Organization)
            .Include(query => query.DefaultedByCustomers);
}

public class LocationRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

    public async Task<ICollection<Location>> GetAllAsync(bool includeDeletedLocationMembers, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers)
            .ToListAsync(cancellationToken);

    public async Task<Location?> GetByIdAsync(string id, bool includeDeletedLocationMembers, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public Location Add(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
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
}
