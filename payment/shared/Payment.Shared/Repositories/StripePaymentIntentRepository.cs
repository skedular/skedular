using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripePaymentIntentRepository : IRepository<StripePaymentIntent>
{
    void Add(StripePaymentIntent stripePaymentIntent);
}

public class StripePaymentIntentRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripePaymentIntent>(dbContext, timeProvider), IStripePaymentIntentRepository
{
    public void Add(StripePaymentIntent stripePaymentIntent)
    {
        var now = TimeProvider.GetUtcNow();
        stripePaymentIntent.CreatedAt = now;
        DbContext.OrganizationOfferingStripePaymentIntent.Add(stripePaymentIntent);
    }
}
