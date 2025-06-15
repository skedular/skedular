using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IFloorPlanRepository : IRepository<Database.Entities.FloorPlan>
{
    Task<Database.Entities.FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
    Task<Database.Entities.FloorPlan?> GetByLocationIdAndFloorLevelAsync(string locationId, int floorLevel, CancellationToken cancellationToken);
    Database.Entities.FloorPlan Add(Database.Entities.FloorPlan floorPlan);
    Database.Entities.FloorPlan Update(Database.Entities.FloorPlan floorPlan);
    Database.Entities.FloorPlan Remove(Database.Entities.FloorPlan floorPlan);
}

public class FloorPlanRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.FloorPlan>(dbContext, timeProvider), IFloorPlanRepository
{
    public async Task<Database.Entities.FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.Location)
            .Include(fp => fp.ResourcePositions)
                .ThenInclude(rp => rp.Resource)
            .FirstOrDefaultAsync(fp => fp.Id == id && !fp.DeletedAt.HasValue, cancellationToken);

    public async Task<ICollection<Database.Entities.FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.ResourcePositions)
                .ThenInclude(rp => rp.Resource)
            .Where(fp => fp.LocationId == locationId && !fp.DeletedAt.HasValue)
            .OrderBy(fp => fp.FloorLevel)
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.FloorPlan?> GetByLocationIdAndFloorLevelAsync(string locationId, int floorLevel, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.ResourcePositions)
                .ThenInclude(rp => rp.Resource)
            .FirstOrDefaultAsync(fp => fp.LocationId == locationId && fp.FloorLevel == floorLevel && !fp.DeletedAt.HasValue, cancellationToken);

    public Database.Entities.FloorPlan Add(Database.Entities.FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.CreatedAt = now;
        return DbContext.FloorPlan.Add(floorPlan).Entity;
    }

    public Database.Entities.FloorPlan Update(Database.Entities.FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.ModifiedAt = now;
        return DbContext.FloorPlan.Update(floorPlan).Entity;
    }

    public Database.Entities.FloorPlan Remove(Database.Entities.FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.DeletedAt = now;
        return DbContext.FloorPlan.Update(floorPlan).Entity;
    }
}