using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

public class LocationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location
            .Include(query => query.Organization)
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
