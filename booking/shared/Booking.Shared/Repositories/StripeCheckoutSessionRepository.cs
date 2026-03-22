using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IStripeCheckoutSessionRepository : IRepository<StripeCheckoutSession>
{
    Task<StripeCheckoutSession?> GetByStripeCheckoutSessionIdAsync(string stripeCheckoutSessionId, CancellationToken cancellationToken);
    StripeCheckoutSession Add(StripeCheckoutSession stripeCheckoutSession);
    StripeCheckoutSession Update(StripeCheckoutSession stripeCheckoutSession);
}

public class StripeCheckoutSessionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, StripeCheckoutSession>(dbContext, timeProvider), IStripeCheckoutSessionRepository
{
    public async Task<StripeCheckoutSession?> GetByStripeCheckoutSessionIdAsync(
        string stripeCheckoutSessionId,
        CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession
            .AsSingleQuery()
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query.Booking)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBookingSubscription)
            .FirstOrDefaultAsync(query => query.StripeCheckoutSessionId == stripeCheckoutSessionId, cancellationToken);

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
