using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IStripeProductRepository : IRepository<StripeProduct>
{
    StripeProduct Add(StripeProduct stripeProduct);
    StripeProduct Update(StripeProduct stripeProduct);
}

public class StripeProductRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, StripeProduct>(dbContext, timeProvider), IStripeProductRepository
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
