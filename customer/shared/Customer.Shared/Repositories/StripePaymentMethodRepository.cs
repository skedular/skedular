using Api.Shared.Services.Cache;
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
    ValueTask AddAsync(StripePaymentMethod stripePaymentMethod, CancellationToken cancellationToken);
    ValueTask RemoveAsync(StripePaymentMethod stripePaymentMethod, CancellationToken cancellationToken);
}

internal static class StripePaymentMethodExtensions
{
    internal static IIncludableQueryable<StripePaymentMethod, ICollection<Identity>> AddDependentObjects(
        this IQueryable<StripePaymentMethod> originalQuery) =>
        originalQuery
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities);
}

public class StripePaymentMethodRepository(
    CustomerDbContext dbContext,
    TimeProvider timeProvider,
    IGenericCustomerCacheService genericCustomerCacheService)
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

    public async ValueTask AddAsync(StripePaymentMethod stripePaymentMethod, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.CreatedAt = now;
        DbContext.StripePaymentMethod.Add(stripePaymentMethod);

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(
            stripePaymentMethod.Customer.Identities.Select(identity => identity.Id),
            cancellationToken);
    }

    public async ValueTask RemoveAsync(StripePaymentMethod stripePaymentMethod, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentMethod.DeletedAt = now;
        DbContext.StripePaymentMethod.Update(stripePaymentMethod);

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(
            stripePaymentMethod.Customer.Identities.Select(identity => identity.Id),
            cancellationToken);
    }
}
