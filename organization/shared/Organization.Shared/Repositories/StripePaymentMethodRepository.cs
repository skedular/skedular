using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IStripePaymentMethodRepository : IRepository<StripePaymentMethod>
{
    Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken);
    void Add(StripePaymentMethod stripePaymentMethod);
    void Remove(StripePaymentMethod stripePaymentMethod);
}

internal static class StripePaymentMethodExtensions
{
    internal static IIncludableQueryable<StripePaymentMethod, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<StripePaymentMethod> originalQuery) =>
        originalQuery
            .Include(query => query.Organization);
}

public class StripePaymentMethodRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, StripePaymentMethod>(dbContext, timeProvider), IStripePaymentMethodRepository
{
    public async Task<StripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripePaymentMethod
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
