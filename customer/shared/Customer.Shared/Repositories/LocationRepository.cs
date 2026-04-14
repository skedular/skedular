using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Customer.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, bool includeDeletedLocationMembers, CancellationToken cancellationToken);
    Location Update(Location location);
    Location Remove(Location location);
}

internal static class LocationExtensions
{
    extension(IQueryable<Location> originalQuery)
    {
        internal IIncludableQueryable<Location, ICollection<Database.Entities.Customer>> AddDependentObjects(bool includeDeletedLocationMembers) =>
            originalQuery
                .Include(query => query.Resources)
                .Include(query => query.Organization)
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.FavouredByCustomers);
    }
}

public class LocationRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, true, cancellationToken))!;
    }

    public async Task<Location?> GetByIdAsync(string id, bool includeDeletedLocationMembers, CancellationToken cancellationToken) =>
        await DbContext.Location
            .AddDependentObjects(includeDeletedLocationMembers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
