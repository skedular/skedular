using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IFloorPlanRepository : IRepository<FloorPlan>
{
    Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
    Task<FloorPlan?> GetByLocationIdAndFloorLevelAsync(string locationId, int floorLevel, CancellationToken cancellationToken);
    FloorPlan Add(FloorPlan floorPlan);
    FloorPlan Update(FloorPlan floorPlan);
    FloorPlan Remove(FloorPlan floorPlan);
}

public class FloorPlanRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, FloorPlan>(dbContext, timeProvider), IFloorPlanRepository
{
    public async Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.Location)
            .Include(fp => fp.ResourcePositions)
            .ThenInclude(rp => rp.Resource)
            .FirstOrDefaultAsync(fp => fp.Id == id && !fp.DeletedAt.HasValue, cancellationToken);

    public async Task<ICollection<FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.ResourcePositions)
            .ThenInclude(rp => rp.Resource)
            .Where(fp => fp.LocationId == locationId && !fp.DeletedAt.HasValue)
            .OrderBy(fp => fp.FloorLevel)
            .ToListAsync(cancellationToken);

    public async Task<FloorPlan?> GetByLocationIdAndFloorLevelAsync(string locationId, int floorLevel, CancellationToken cancellationToken) =>
        await DbContext.FloorPlan
            .Include(fp => fp.ResourcePositions)
            .ThenInclude(rp => rp.Resource)
            .FirstOrDefaultAsync(fp => fp.LocationId == locationId && fp.FloorLevel == floorLevel && !fp.DeletedAt.HasValue, cancellationToken);

    public FloorPlan Add(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.CreatedAt = now;
        return DbContext.FloorPlan.Add(floorPlan).Entity;
    }

    public FloorPlan Update(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.ModifiedAt = now;
        return DbContext.FloorPlan.Update(floorPlan).Entity;
    }

    public FloorPlan Remove(FloorPlan floorPlan)
    {
        var now = TimeProvider.GetUtcNow();
        floorPlan.DeletedAt = now;
        return DbContext.FloorPlan.Update(floorPlan).Entity;
    }
}
