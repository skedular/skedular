using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;

namespace Customer.Shared.Repositories;

public interface IStripePaymentIntentRepository : IRepository<StripePaymentIntent>
{
    void Add(StripePaymentIntent stripePaymentIntent);
}

public class StripePaymentIntentRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, StripePaymentIntent>(dbContext, timeProvider), IStripePaymentIntentRepository
{
    public void Add(StripePaymentIntent stripePaymentIntent)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentIntent.CreatedAt = now;
        DbContext.StripePaymentIntent.Add(stripePaymentIntent);
    }
}
