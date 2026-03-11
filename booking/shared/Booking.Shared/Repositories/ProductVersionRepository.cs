using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Booking.Shared.Repositories;

public interface IProductVersionRepository : IRepository<ProductVersion>
{
    Task<ProductVersion> UpsertNakedAsync(string id, Product? product, CancellationToken cancellationToken);
    Task<ProductVersion?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ProductVersion Update(ProductVersion product);
}

internal static class ProductVersionExtensions
{
    extension(IQueryable<ProductVersion> originalQuery)
    {
        internal IIncludableQueryable<ProductVersion, StripePrice?> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Product)
                .ThenInclude(query => query.Organization)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.StripeProducts)
                .ThenInclude(query => query.StripePrice);
    }
}

public class ProductVersionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, ProductVersion>(dbContext, timeProvider), IProductVersionRepository
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
