using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ProductVersion Add(ProductVersion product);
    ProductVersion Update(ProductVersion product);
}

internal static class ProductVersionExtensions
{
    internal static IIncludableQueryable<ProductVersion, StripePrice?> AddDependentObjects(
        this IQueryable<ProductVersion> originalQuery) =>
        originalQuery
            .Include(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.StripeProduct)
            .Include(query => query.StripePrice);
}

public class ProductVersionRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
{
    public async Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.ProductVersion
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public ProductVersion Add(ProductVersion product)
    {
        var now = TimeProvider.GetUtcNow();
        product.CreatedAt = now;
        return DbContext.ProductVersion.Add(product).Entity;
    }

    public ProductVersion Update(ProductVersion product)
    {
        var now = TimeProvider.GetUtcNow();
        product.ModifiedAt = now;
        return DbContext.ProductVersion.Update(product).Entity;
    }
}
