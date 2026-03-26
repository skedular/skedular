using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Location Add(Location location);
    Location Update(Location location);
    Location Remove(Location location);
}

public class LocationRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Location>(dbContext, timeProvider), ILocationRepository
{
    public async Task<Location?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Location.FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public Location Add(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.CreatedAt = now;
        return DbContext.Location.Add(location).Entity;
    }

    public Location Update(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public Location Remove(Location location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Location.Update(location).Entity;
    }

    public override async Task<Location> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }
}
