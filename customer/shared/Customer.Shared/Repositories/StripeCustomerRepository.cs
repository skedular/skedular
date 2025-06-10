using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Customer.Shared.Repositories;

public interface IStripeCustomerRepository : IRepository<StripeCustomer>
{
    StripeCustomer Add(StripeCustomer stripeCustomer);
}

public class StripeCustomerRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, StripeCustomer>(dbContext, timeProvider), IStripeCustomerRepository
{
    public StripeCustomer Add(StripeCustomer stripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCustomer.CreatedAt = now;
        return DbContext.StripeCustomer.Add(stripeCustomer).Entity;
    }
}
