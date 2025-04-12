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
    Task<ICollection<Product>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<ICollection<Product>> GetAllAsync(CancellationToken cancellationToken);
    Product Add(Product product);
    Product Update(Product product);
    Product Remove(Product product);
    void RemoveRange(ICollection<Product> products);

    Task<(PaginatedInfo, ICollection<Edge<Product>>, int )> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken);
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

    internal static IQueryable<Product> AddSearchCriteria(this IQueryable<Product> query, ProductSearchCriteria searchCriteria)
    {
        query = query.Where(item =>
            !item.DeletedAt.HasValue && item.Organization.Type == OrganizationTypeConstants.Marketplace &&
            (searchCriteria.IncludeInactive || !item.Inactive));

        if (searchCriteria.OrganizationIds.Count > 0)
        {
            query = query.Where(item => !item.Organization.DeletedAt.HasValue && searchCriteria.OrganizationIds.Contains(item.Organization.Id));
        }

        if (searchCriteria.ProductIds.Count > 0)
        {
            query = query.Where(item => searchCriteria.ProductIds.Contains(item.Id));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Product> AddSortingOrders(this IQueryable<Product> originalQuery, ICollection<ProductOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            ProductOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                ProductOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class ProductRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, Product>(dbContext, timeProvider), IProductRepository
{
    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Product
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Product>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Product
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

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

    public void RemoveRange(ICollection<Product> products)
    {
        var now = TimeProvider.GetUtcNow();
        products.ForEach(product => product.DeletedAt = now);
        DbContext.Product.UpdateRange(products);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Product
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
