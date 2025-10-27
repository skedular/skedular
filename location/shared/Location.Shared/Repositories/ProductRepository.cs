using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Product Update(Product product);
    Product Remove(Product product);
}

internal static class ProductExtensions
{
    internal static IIncludableQueryable<Product, IEnumerable<OrganizationTag>> AddDependentObjects(this IQueryable<Product> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt))
            .ThenInclude(query => query.ProductTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt))
            .ThenInclude(query => query.LocationTags.Where(tag => !tag.DeletedAt.HasValue));
}

public class ProductRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Product>(dbContext, timeProvider), IProductRepository
{
    public async Task<Product> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Product
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
