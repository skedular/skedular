using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Location.Shared.Repositories;

public interface IResourcePositionRepository : IRepository<Database.Entities.ResourcePosition>
{
    Task<Database.Entities.ResourcePosition?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Database.Entities.ResourcePosition?> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.ResourcePosition>> GetByFloorPlanIdAsync(string floorPlanId, CancellationToken cancellationToken);
    Database.Entities.ResourcePosition Add(Database.Entities.ResourcePosition resourcePosition);
    Database.Entities.ResourcePosition Update(Database.Entities.ResourcePosition resourcePosition);
    void Remove(Database.Entities.ResourcePosition resourcePosition);
}

public class ResourcePositionRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Database.Entities.ResourcePosition>(dbContext, timeProvider), IResourcePositionRepository
{
    public async Task<Database.Entities.ResourcePosition?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Include(rp => rp.FloorPlan)
            .FirstOrDefaultAsync(rp => rp.Id == id, cancellationToken);

    public async Task<Database.Entities.ResourcePosition?> GetByResourceIdAsync(string resourceId, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Include(rp => rp.FloorPlan)
            .FirstOrDefaultAsync(rp => rp.ResourceId == resourceId, cancellationToken);

    public async Task<ICollection<Database.Entities.ResourcePosition>> GetByFloorPlanIdAsync(string floorPlanId, CancellationToken cancellationToken) =>
        await DbContext.ResourcePosition
            .Include(rp => rp.Resource)
            .Where(rp => rp.FloorPlanId == floorPlanId)
            .ToListAsync(cancellationToken);

    public Database.Entities.ResourcePosition Add(Database.Entities.ResourcePosition resourcePosition)
    {
        var now = TimeProvider.GetUtcNow();
        resourcePosition.CreatedAt = now;
        return DbContext.ResourcePosition.Add(resourcePosition).Entity;
    }

    public Database.Entities.ResourcePosition Update(Database.Entities.ResourcePosition resourcePosition)
    {
        var now = TimeProvider.GetUtcNow();
        resourcePosition.ModifiedAt = now;
        return DbContext.ResourcePosition.Update(resourcePosition).Entity;
    }

    public void Remove(Database.Entities.ResourcePosition resourcePosition)
    {
        DbContext.ResourcePosition.Remove(resourcePosition);
    }
}