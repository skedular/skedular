using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion> UpsertNakedAsync(string id, Product? product, CancellationToken cancellationToken);
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ProductVersion Update(ProductVersion product);
}

internal static class ProductVersionExtensions
{
    internal static IIncludableQueryable<ProductVersion, OrganizationStripeConnectAccount?> AddDependentObjects(
        this IQueryable<ProductVersion> originalQuery) =>
        originalQuery
            .Include(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.OrganizationStripeConnectAccount);
}

public class ProductVersionRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
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

    public ProductVersion Update(ProductVersion product)
    {
        var now = TimeProvider.GetUtcNow();
        product.ModifiedAt = now;
        return DbContext.ProductVersion.Update(product).Entity;
    }
}
