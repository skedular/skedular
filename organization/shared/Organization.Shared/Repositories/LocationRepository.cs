using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(
        string id,
        Database.Entities.Organization organization,
        CancellationToken cancellationToken);

    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

public class LocationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(
        string id,
        Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Database.Entities.Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

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
