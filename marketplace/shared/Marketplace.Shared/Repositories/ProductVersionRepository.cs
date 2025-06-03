using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ProductVersion Add(ProductVersion productVersion);
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

public class ProductVersionRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
{
    public async Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ProductVersion
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public ProductVersion Add(ProductVersion productVersion)
    {
        var now = TimeProvider.GetUtcNow();
        productVersion.CreatedAt = now;
        return DbContext.ProductVersion.Add(productVersion).Entity;
    }
}
