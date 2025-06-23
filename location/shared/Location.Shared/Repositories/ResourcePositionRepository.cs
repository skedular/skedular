using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IResourcePositionRepository : IRepository<ResourcePosition>
{
    ResourcePosition Add(ResourcePosition resourcePosition);
    ResourcePosition Update(ResourcePosition resourcePosition);
    void RemoveRange(IEnumerable<ResourcePosition> resourcePositions);
}

public class ResourcePositionRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, ResourcePosition>(dbContext, timeProvider), IResourcePositionRepository
{
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

    public void RemoveRange(IEnumerable<ResourcePosition> resourcePositions) => DbContext.ResourcePosition.RemoveRange(resourcePositions);
}
