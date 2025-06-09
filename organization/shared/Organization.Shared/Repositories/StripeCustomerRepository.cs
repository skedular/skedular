using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IStripeCustomerRepository : IRepository<StripeCustomer>
{
    StripeCustomer Add(StripeCustomer stripeCustomer);
}

public class StripeCustomerRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, StripeCustomer>(dbContext, timeProvider), IStripeCustomerRepository
{
    public StripeCustomer Add(StripeCustomer stripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCustomer.CreatedAt = now;
        return DbContext.StripeCustomer.Add(stripeCustomer).Entity;
    }
}
