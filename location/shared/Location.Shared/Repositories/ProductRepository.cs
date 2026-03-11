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
    Task<ICollection<Product>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Product Update(Product product);
    Product Remove(Product product);
}

internal static class ProductExtensions
{
    extension(IQueryable<Product> originalQuery)
    {
        internal IIncludableQueryable<Product, IEnumerable<OrganizationTag>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Organization)
                .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt))
                .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue));
    }
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

    public async Task<ICollection<Product>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

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
