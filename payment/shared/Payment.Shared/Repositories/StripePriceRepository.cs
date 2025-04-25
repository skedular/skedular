using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripePriceRepository : IRepository<StripePrice>
{
    StripePrice Add(StripePrice stripePrice);
}

public class StripePriceRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripePrice>(dbContext, timeProvider), IStripePriceRepository
{
    public StripePrice Add(StripePrice stripePrice)
    {
        var now = TimeProvider.GetUtcNow();
        stripePrice.CreatedAt = now;
        return DbContext.StripePrice.Add(stripePrice).Entity;
    }
}
