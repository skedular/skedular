using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripePaymentMethodRepository : IRepository<OrganizationStripePaymentMethod>
{
    Task<OrganizationStripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken);
    void Add(OrganizationStripePaymentMethod organizationStripePaymentMethod);
    void Remove(OrganizationStripePaymentMethod organizationStripePaymentMethod);
}

internal static class StripePaymentMethodExtensions
{
    internal static IIncludableQueryable<OrganizationStripePaymentMethod, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<OrganizationStripePaymentMethod> originalQuery) =>
        originalQuery
            .Include(query => query.Organization);
}

public class OrganizationStripePaymentMethodRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripePaymentMethod>(dbContext, timeProvider), IOrganizationStripePaymentMethodRepository
{
    public async Task<OrganizationStripePaymentMethod?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripePaymentMethod
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public void Add(OrganizationStripePaymentMethod organizationStripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethod.CreatedAt = now;
        DbContext.OrganizationStripePaymentMethod.Add(organizationStripePaymentMethod);
    }

    public void Remove(OrganizationStripePaymentMethod organizationStripePaymentMethod)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentMethod.DeletedAt = now;
        DbContext.OrganizationStripePaymentMethod.Update(organizationStripePaymentMethod);
    }
}
