using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using LocationPhysicalAddress = Location.Shared.Database.Entities.LocationPhysicalAddress;

namespace Location.Shared.Repositories;

public interface ILocationPhysicalAddressRepository : IRepository<LocationPhysicalAddress>
{
    Task<LocationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken);
    LocationPhysicalAddress Add(LocationPhysicalAddress address);
    LocationPhysicalAddress Update(LocationPhysicalAddress address);
    LocationPhysicalAddress Remove(LocationPhysicalAddress address);
}

internal static class LocationPhysicalAddressExtensions
{
    internal static IIncludableQueryable<LocationPhysicalAddress, Database.Entities.Location> AddDependentObjects(
        this IQueryable<LocationPhysicalAddress> originalQuery) =>
        originalQuery
            .Include(query => query.Location);
}

public class LocationPhysicalAddressRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, LocationPhysicalAddress>(dbContext, timeProvider), ILocationPhysicalAddressRepository
{
    public async Task<LocationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.LocationPhysicalAddress
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public LocationPhysicalAddress Add(LocationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        return DbContext.LocationPhysicalAddress.Add(address).Entity;
    }

    public LocationPhysicalAddress Update(LocationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        return DbContext.LocationPhysicalAddress.Update(address).Entity;
    }

    public LocationPhysicalAddress Remove(LocationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.DeletedAt = now;
        return DbContext.LocationPhysicalAddress.Update(address).Entity;
    }
}
