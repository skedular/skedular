using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(ICollection<Resource> resources);
    Task<ICollection<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
}

public class ResourceRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Resource>(dbContext, timeProvider), IResourceRepository
{
    public async Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public Resource Add(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.CreatedAt = now;
        return DbContext.Resource.Add(resource).Entity;
    }

    public void RemoveRange(ICollection<Resource> resources)
    {
        var now = TimeProvider.GetUtcNow();
        resources.ForEach(resource => resource.DeletedAt = now);
        DbContext.Resource.UpdateRange(resources);
    }

    public Resource Update(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.ModifiedAt = now;
        return DbContext.Resource.Update(resource).Entity;
    }

    public async Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Resource
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.Resource
                .Include(query => query.Location)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Include(query => query.Location)
            .Where(query => query.Location != null && query.Location.Id == locationId)
            .ToListAsync(cancellationToken);
}
