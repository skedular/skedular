using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;

namespace Booking.Shared.Repositories;

public interface IStripePriceRepository : IRepository<StripePrice>
{
    StripePrice Add(StripePrice stripePrice);
}

public class StripePriceRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, StripePrice>(dbContext, timeProvider), IStripePriceRepository
{
    public StripePrice Add(StripePrice stripePrice)
    {
        var now = TimeProvider.GetUtcNow();
        stripePrice.CreatedAt = now;
        return DbContext.StripePrice.Add(stripePrice).Entity;
    }
}
