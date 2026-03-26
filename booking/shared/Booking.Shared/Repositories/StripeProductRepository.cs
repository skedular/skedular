using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;

namespace Booking.Shared.Repositories;

public interface IStripeProductRepository : IRepository<StripeProduct>
{
    StripeProduct Add(StripeProduct stripeProduct);
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
}
