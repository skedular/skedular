using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripePaymentIntentRepository : IRepository<OrganizationStripePaymentIntent>
{
    void Add(OrganizationStripePaymentIntent organizationStripePaymentIntent);
}

public class OrganizationOrganizationStripePaymentIntentRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripePaymentIntent>(dbContext, timeProvider), IOrganizationStripePaymentIntentRepository
{
    public void Add(OrganizationStripePaymentIntent organizationStripePaymentIntent)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripePaymentIntent.CreatedAt = now;
        DbContext.OrganizationStripePaymentIntent.Add(organizationStripePaymentIntent);
    }
}
