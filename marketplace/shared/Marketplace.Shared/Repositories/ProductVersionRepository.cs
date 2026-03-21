using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    ProductVersion Add(ProductVersion productVersion);
}

internal static class ProductVersionExtensions
{
    extension(IQueryable<ProductVersion> originalQuery)
    {
        internal IIncludableQueryable<ProductVersion, IEnumerable<OrganizationTag>> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTracking())
            .Include(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));
    }
}

public class ProductVersionRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
{
    public async Task<ProductVersion?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ProductVersion
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public ProductVersion Add(ProductVersion productVersion)
    {
        var now = TimeProvider.GetUtcNow();
        productVersion.CreatedAt = now;
        return DbContext.ProductVersion.Add(productVersion).Entity;
    }
}
