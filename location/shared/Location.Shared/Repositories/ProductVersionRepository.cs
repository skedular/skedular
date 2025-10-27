using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion> UpsertNakedAsync(string id, Product? product, CancellationToken cancellationToken);
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<ProductVersion>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
    ProductVersion Update(ProductVersion product);
}

internal static class ProductVersionExtensions
{
    internal static IIncludableQueryable<ProductVersion, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<ProductVersion> originalQuery) =>
        originalQuery
            .Include(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.ProductTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.LocationTags.Where(tag => !tag.DeletedAt.HasValue));
}

public class ProductVersionRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
{
    public async Task<ProductVersion> UpsertNakedAsync(string id, Product? product, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Product>(id, product, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ProductVersion
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<ProductVersion>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken) =>
        await DbContext.ProductVersion
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public ProductVersion Update(ProductVersion product)
    {
        var now = TimeProvider.GetUtcNow();
        product.ModifiedAt = now;
        return DbContext.ProductVersion.Update(product).Entity;
    }
}
