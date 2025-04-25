using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeCustomerRepository : IRepository<StripeCustomer>
{
    StripeCustomer Add(StripeCustomer stripeCustomer);
    StripeCustomer Update(StripeCustomer stripeCustomer);
}

public class StripeCustomerRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeCustomer>(dbContext, timeProvider), IStripeCustomerRepository
{
    public StripeCustomer Add(StripeCustomer stripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCustomer.CreatedAt = now;
        return DbContext.StripeCustomer.Add(stripeCustomer).Entity;
    }

    public StripeCustomer Update(StripeCustomer stripeCustomer)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCustomer.ModifiedAt = now;
        return DbContext.StripeCustomer.Update(stripeCustomer).Entity;
    }
}
