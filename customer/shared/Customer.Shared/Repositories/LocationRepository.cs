using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

public class LocationRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Location>(dbContext), ILocationRepository
{
    public async Task<Location>
        UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Location.Add(new Location { Id = id, CreatedAt = now, Organization = organization }).Entity;
    }

    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => query.Id == id)
            .Include(query =>
                query.LocationMembers.Where(locationMember => !locationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Desks)
            .Include(query => query.Organization)
            .Include(query => query.DefaultedByCustomers)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Location Add(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Location Remove(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = timeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }
}
