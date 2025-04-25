using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeProductRepository : IRepository<StripeProduct>
{
    StripeProduct Add(StripeProduct stripeProduct);
    StripeProduct Update(StripeProduct stripeProduct);
}

public class StripeProductRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeProduct>(dbContext, timeProvider), IStripeProductRepository
{
    public StripeProduct Add(StripeProduct stripeProduct)
    {
        var now = TimeProvider.GetUtcNow();
        stripeProduct.CreatedAt = now;
        return DbContext.StripeProduct.Add(stripeProduct).Entity;
    }

    public StripeProduct Update(StripeProduct stripeProduct)
    {
        var now = TimeProvider.GetUtcNow();
        stripeProduct.ModifiedAt = now;
        return DbContext.StripeProduct.Update(stripeProduct).Entity;
    }
}
