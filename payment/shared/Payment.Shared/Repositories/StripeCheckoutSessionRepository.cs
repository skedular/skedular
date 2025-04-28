using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeCheckoutSessionRepository : IRepository<StripeCheckoutSession>
{
    StripeCheckoutSession Add(StripeCheckoutSession stripeCheckoutSession);
    StripeCheckoutSession Update(StripeCheckoutSession stripeCheckoutSession);
}

public class StripeCheckoutSessionRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeCheckoutSession>(dbContext, timeProvider), IStripeCheckoutSessionRepository
{
    public StripeCheckoutSession Add(StripeCheckoutSession stripeCheckoutSession)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCheckoutSession.CreatedAt = now;
        return DbContext.StripeCheckoutSession.Add(stripeCheckoutSession).Entity;
    }

    public StripeCheckoutSession Update(StripeCheckoutSession stripeCheckoutSession)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCheckoutSession.ModifiedAt = now;
        return DbContext.StripeCheckoutSession.Update(stripeCheckoutSession).Entity;
    }
}
