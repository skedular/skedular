using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllAsync(CancellationToken cancellationToken);
    Product Add(Product product);
    Product Update(Product product);
    Product Remove(Product product);
}

internal static class ProductExtensions
{
    internal static IIncludableQueryable<Product, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Product> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt).First())
            .ThenInclude(query => query.ProductTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt).First())
            .ThenInclude(query => query.LocationTags.Where(tag => !tag.DeletedAt.HasValue));
}

public class ProductRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, Product>(dbContext, timeProvider), IProductRepository
{
    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Product
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Product>> GetAllByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Product Add(Product product)
    {
        var now = TimeProvider.GetUtcNow();
        product.CreatedAt = now;
        return DbContext.Product.Add(product).Entity;
    }

    public Product Update(Product product)
    {
        var now = TimeProvider.GetUtcNow();
        product.ModifiedAt = now;
        return DbContext.Product.Update(product).Entity;
    }

    public Product Remove(Product product)
    {
        var now = TimeProvider.GetUtcNow();
        product.DeletedAt = now;
        return DbContext.Product.Update(product).Entity;
    }
}
