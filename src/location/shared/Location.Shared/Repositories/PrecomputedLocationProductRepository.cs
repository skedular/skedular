using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IPrecomputedLocationProductRepository : IRepository<PrecomputedLocationProduct>
{
    Task<IReadOnlyList<PrecomputedLocationProduct>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PrecomputedLocationProduct>> GetByLocationAsync(string locationId, CancellationToken cancellationToken);
    PrecomputedLocationProduct Add(PrecomputedLocationProduct precomputedLocationProduct);
    void RemoveRange(IEnumerable<PrecomputedLocationProduct> precomputedLocationProducts);
}

public static class PrecomputedLocationProductExtensions
{
    extension(IQueryable<PrecomputedLocationProduct> originalQuery)
    {
        public IIncludableQueryable<PrecomputedLocationProduct, Product> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Organization)
            .Include(query => query.Location)
            .ThenInclude(query => query.Organization)
            .Include(query => query.Location)
            .ThenInclude(query => query.PhysicalAddress)
            .Include(query => query.OrganizationTags)
            .Include(query => query.Product);
    }
}

public class PrecomputedLocationProductRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, PrecomputedLocationProduct>(dbContext, timeProvider), IPrecomputedLocationProductRepository
{
    public async Task<IReadOnlyList<PrecomputedLocationProduct>>
        GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.PrecomputedLocationProduct
            .Where(query => query.Organization.Id == organizationId)
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PrecomputedLocationProduct>> GetByLocationAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.PrecomputedLocationProduct
            .Where(query => query.Location.Id == locationId)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public PrecomputedLocationProduct Add(PrecomputedLocationProduct precomputedLocationProduct)
    {
        var now = TimeProvider.GetUtcNow();
        precomputedLocationProduct.CreatedAt = now;
        return DbContext.PrecomputedLocationProduct.Add(precomputedLocationProduct).Entity;
    }

    public void RemoveRange(IEnumerable<PrecomputedLocationProduct> precomputedLocationProducts) =>
        DbContext.PrecomputedLocationProduct.RemoveRange(precomputedLocationProducts);
}
