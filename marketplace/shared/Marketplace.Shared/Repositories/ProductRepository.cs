using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Marketplace.Shared.Database;
using Marketplace.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;
using Product = Marketplace.Shared.Database.Entities.Product;

namespace Marketplace.Shared.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Product?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllUntrackedAsync(CancellationToken cancellationToken);
    Product Add(Product product);
    Product Update(Product product);
    void RemoveRange(ICollection<Product> products);

    Task<(PaginatedInfo, ICollection<Edge<Product>>, int )> GetPaginatedProductsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class ProductExtensions
{
    extension(IQueryable<Product> originalQuery)
    {
        internal IIncludableQueryable<Product, IEnumerable<OrganizationTag>> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.ProductVersions.OrderByDescending(productVersion => productVersion.CreatedAt).Take(1))
            .ThenInclude(query => query.ProductTags.Where(tag => !tag.DeletedAt.HasValue));

        internal IQueryable<Product> AddSearchCriteria(ProductSearchCriteria searchCriteria)
        {
            originalQuery = originalQuery.Where(item =>
                !item.DeletedAt.HasValue && item.Organization.Type == OrganizationTypeConstants.Marketplace &&
                (searchCriteria.IncludeInactive || !item.Inactive));

            if (searchCriteria.OrganizationUniqueAlphanumericNames.Count > 0)
            {
                originalQuery = originalQuery.Where(item =>
                    !item.Organization.DeletedAt.HasValue && item.Organization.UniqueAlphanumericName != null &&
                    searchCriteria.OrganizationUniqueAlphanumericNames.Contains(item.Organization.UniqueAlphanumericName));
            }

            if (searchCriteria.ProductIds.Count > 0)
            {
                originalQuery = originalQuery.Where(item => searchCriteria.ProductIds.Contains(item.Id));
            }

            return originalQuery;
        }

        internal IQueryable<Product> AddSortingOrders(ICollection<ProductOrder> orderByFields) =>
            originalQuery.OrderBy(query => query.Id).ThenBy(query => query.Id);
    }
}

public class ProductRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, Product>(dbContext, timeProvider), IProductRepository
{
    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Product
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Product?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Product
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Product>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Product>> GetAllByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => !query.DeletedAt.HasValue && query.Organization.Id == organizationId)
            .AddDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Product>> GetAllUntrackedAsync(CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false)
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

    public void RemoveRange(ICollection<Product> products)
    {
        var now = TimeProvider.GetUtcNow();
        products.ForEach(product => product.DeletedAt = now);
        DbContext.Product.UpdateRange(products);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Product>>, int)> GetPaginatedProductsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Product
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
