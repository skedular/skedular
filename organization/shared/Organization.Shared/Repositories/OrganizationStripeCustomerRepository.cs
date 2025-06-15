using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripeCustomerRepository : IRepository<OrganizationStripeCustomer>
{
    OrganizationStripeCustomer Add(OrganizationStripeCustomer organizationStripeCustomer);
}

public class OrganizationOrganizationStripeCustomerRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripeCustomer>(dbContext, timeProvider), IOrganizationStripeCustomerRepository
{
    public OrganizationStripeCustomer Add(OrganizationStripeCustomer organizationStripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeCustomer.CreatedAt = now;
        return DbContext.OrganizationStripeCustomer.Add(organizationStripeCustomer).Entity;
    }
}
