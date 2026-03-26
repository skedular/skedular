using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Customer.Shared.Repositories;

public interface IStripePaymentMethodRepository : IRepository<StripePaymentMethod>
{
    Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<StripePaymentMethod>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    void Add(StripePaymentMethod stripePaymentMethod);
    void Remove(StripePaymentMethod stripePaymentMethod);
}

internal static class StripePaymentMethodExtensions
{
    extension(IQueryable<StripePaymentMethod> originalQuery)
    {
        internal IIncludableQueryable<StripePaymentMethod, ICollection<Identity>> AddDependentObjects() =>
            originalQuery
                .AsSingleQuery()
                .Include(query => query.Customer)
                .ThenInclude(query => query.Identities);
    }
}

public class StripePaymentMethodRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, StripePaymentMethod>(dbContext, timeProvider), IStripePaymentMethodRepository
{
    public async Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripePaymentMethod
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<StripePaymentMethod>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.StripePaymentMethod
            .Where(query => !query.DeletedAt.HasValue && query.Customer.Id == customerId)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public void Add(StripePaymentMethod stripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.CreatedAt = now;
        DbContext.StripePaymentMethod.Add(stripePaymentMethod);
    }

    public void Remove(StripePaymentMethod stripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.DeletedAt = now;
        DbContext.StripePaymentMethod.Update(stripePaymentMethod);
    }
}
