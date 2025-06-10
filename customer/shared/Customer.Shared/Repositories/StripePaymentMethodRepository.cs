using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Customer.Shared.Repositories;

public interface IStripePaymentMethodRepository : IRepository<StripePaymentMethod>
{
    Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<StripePaymentMethod?> GetBySetupIntentIdAsync(string setupIntentId, CancellationToken cancellationToken);
    void Add(StripePaymentMethod stripePaymentMethod);
    void Remove(StripePaymentMethod stripePaymentMethod);
}

internal static class StripePaymentMethodExtensions
{
    internal static IIncludableQueryable<StripePaymentMethod, Database.Entities.Customer> AddDependentObjects(
        this IQueryable<StripePaymentMethod> originalQuery) =>
        originalQuery
            .Include(query => query.Customer);
}

public class StripePaymentMethodRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, StripePaymentMethod>(dbContext, timeProvider), IStripePaymentMethodRepository
{
    public async Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripePaymentMethod
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<StripePaymentMethod?> GetBySetupIntentIdAsync(string setupIntentId, CancellationToken cancellationToken) =>
        await DbContext.StripePaymentMethod
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.SetupIntentId == setupIntentId, cancellationToken);

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
