using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IResourcePositionRepository : IRepository<ResourcePosition>
{
    Task<ResourcePosition?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ResourcePosition?> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken);
    Task<ICollection<ResourcePosition>> GetByFloorPlanIdAsync(string floorPlanId, CancellationToken cancellationToken);
    ResourcePosition Add(ResourcePosition resourcePosition);
    ResourcePosition Update(ResourcePosition resourcePosition);
    void Remove(ResourcePosition resourcePosition);
}

public class ResourcePositionRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, ResourcePosition>(dbContext, timeProvider), IResourcePositionRepository
{
    public async Task<ResourcePosition?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Include(rp => rp.FloorPlan)
            .FirstOrDefaultAsync(rp => rp.Id == id, cancellationToken);

    public async Task<ResourcePosition?> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Include(rp => rp.FloorPlan)
            .FirstOrDefaultAsync(rp => rp.ResourceId == resourceId, cancellationToken);

    public async Task<ICollection<ResourcePosition>> GetByFloorPlanIdAsync(string floorPlanId, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Where(rp => rp.FloorPlanId == floorPlanId)
            .ToListAsync(cancellationToken);

    public ResourcePosition Add(ResourcePosition resourcePosition)
    {
        var now = TimeProvider.GetUtcNow();
        resourcePosition.CreatedAt = now;
        return DbContext.ResourcePosition.Add(resourcePosition).Entity;
    }

    public ResourcePosition Update(ResourcePosition resourcePosition)
    {
        var now = TimeProvider.GetUtcNow();
        resourcePosition.ModifiedAt = now;
        return DbContext.ResourcePosition.Update(resourcePosition).Entity;
    }

    public void Remove(ResourcePosition resourcePosition) => DbContext.ResourcePosition.Remove(resourcePosition);
}
