using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ILocationResourceRepository : IRepository<LocationResource>
{
    Task<LocationResource> UpsertNakedAsync(
        string id,
        Location location,
        OrganizationResourceType organizationResourceType,
        CancellationToken cancellationToken);

    Task<LocationResource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    LocationResource Add(LocationResource locationResource);
    LocationResource Update(LocationResource locationResource);
    void RemoveRange(ICollection<LocationResource> locationResources);
    Task<ICollection<LocationResource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationResourceRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, LocationResource>(dbContext, timeProvider), ILocationResourceRepository
{
    public async Task<LocationResource> UpsertNakedAsync(
        string id,
        Location location,
        OrganizationResourceType organizationResourceType,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location, OrganizationResourceType>(id, location, organizationResourceType, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public LocationResource Add(LocationResource locationResource)
    {
        var now = TimeProvider.GetUtcNow();
        locationResource.CreatedAt = now;
        return DbContext.LocationResource.Add(locationResource).Entity;
    }

    public void RemoveRange(ICollection<LocationResource> locationResources)
    {
        var now = TimeProvider.GetUtcNow();
        locationResources.ForEach(locationResource => locationResource.DeletedAt = now);
        DbContext.LocationResource.UpdateRange(locationResources);
    }

    public LocationResource Update(LocationResource locationResource)
    {
        var now = TimeProvider.GetUtcNow();
        locationResource.ModifiedAt = now;
        return DbContext.LocationResource.Update(locationResource).Entity;
    }

    public async Task<LocationResource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.LocationResource
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationResourceType)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.LocationResource
                .Include(query => query.Location)
                .Include(query => query.OrganizationResourceType)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<LocationResource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.LocationResource
            .Include(query => query.Location)
            .Include(query => query.OrganizationResourceType)
            .Where(query => query.Location.Id == locationId)
            .ToListAsync(cancellationToken);
}
