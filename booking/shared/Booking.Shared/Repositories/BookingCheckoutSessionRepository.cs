using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IBookingCheckoutSessionRepository : IRepository<BookingCheckoutSession>
{
    BookingCheckoutSession Add(BookingCheckoutSession stripeProduct);
    BookingCheckoutSession Update(BookingCheckoutSession stripeProduct);
}

public class BookingCheckoutSessionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, BookingCheckoutSession>(dbContext, timeProvider), IBookingCheckoutSessionRepository
{
    public BookingCheckoutSession Add(BookingCheckoutSession stripeProduct)
    {
        var now = TimeProvider.GetUtcNow();
        stripeProduct.CreatedAt = now;
        return DbContext.BookingCheckoutSession.Add(stripeProduct).Entity;
    }

    public BookingCheckoutSession Update(BookingCheckoutSession stripeProduct)
    {
        var now = TimeProvider.GetUtcNow();
        stripeProduct.ModifiedAt = now;
        return DbContext.BookingCheckoutSession.Update(stripeProduct).Entity;
    }
}
