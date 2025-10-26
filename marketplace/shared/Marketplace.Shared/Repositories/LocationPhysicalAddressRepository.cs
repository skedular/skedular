using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface ILocationPhysicalAddressRepository : IRepository<LocationPhysicalAddress>
{
    LocationPhysicalAddress Add(LocationPhysicalAddress address);
    LocationPhysicalAddress Update(LocationPhysicalAddress address);
    void Remove(LocationPhysicalAddress address);
}

internal static class LocationPhysicalAddressExtensions
{
    internal static IIncludableQueryable<LocationPhysicalAddress, Location> AddDependentObjects(
        this IQueryable<LocationPhysicalAddress> originalQuery) =>
        originalQuery
            .Include(query => query.Location);
}

public class LocationPhysicalAddressRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, LocationPhysicalAddress>(dbContext, timeProvider), ILocationPhysicalAddressRepository
{
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

    public void Remove(LocationPhysicalAddress address) => DbContext.LocationPhysicalAddress.Remove(address);
}
