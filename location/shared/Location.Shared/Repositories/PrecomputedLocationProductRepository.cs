using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IPrecomputedLocationProductRepository : IRepository<PrecomputedLocationProduct>
{
    Task<ICollection<PrecomputedLocationProduct>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    PrecomputedLocationProduct Add(PrecomputedLocationProduct precomputedLocationProduct);
    void RemoveRange(ICollection<PrecomputedLocationProduct> precomputedLocationProducts);
}

internal static class PrecomputedLocationProductExtensions
{
    extension(IQueryable<PrecomputedLocationProduct> originalQuery)
    {
        internal IIncludableQueryable<PrecomputedLocationProduct, Product> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
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
    public async Task<ICollection<PrecomputedLocationProduct>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.PrecomputedLocationProduct
            .Where(query => query.Organization.Id == organizationId)
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public PrecomputedLocationProduct Add(PrecomputedLocationProduct precomputedLocationProduct)
    {
        var now = TimeProvider.GetUtcNow();
        precomputedLocationProduct.CreatedAt = now;
        return DbContext.PrecomputedLocationProduct.Add(precomputedLocationProduct).Entity;
    }

    public void RemoveRange(ICollection<PrecomputedLocationProduct> precomputedLocationProducts) =>
        DbContext.PrecomputedLocationProduct.RemoveRange(precomputedLocationProducts);
}
