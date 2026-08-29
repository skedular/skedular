using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IStripeCheckoutSessionRepository : IRepository<StripeCheckoutSession>
{
    Task<StripeCheckoutSession?> GetByStripeCheckoutSessionIdAsync(string stripeCheckoutSessionId, CancellationToken cancellationToken);
    Task<StripeCheckoutSession?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken);
    Task<StripeCheckoutSession?> GetByTransferIdAsync(string transferId, CancellationToken cancellationToken);
    Task<StripeCheckoutSession?> GetByPayoutIdAsync(string payoutId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StripeCheckoutSession>> GetDestinationChargeCandidatesAsync(
        string destinationAccountId, IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken);

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

    public async Task<StripeCheckoutSession?> GetByPaymentIntentIdAsync(
        string paymentIntentId, CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession
            .AsSingleQuery()
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query.Booking)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBookingSubscription)
            .FirstOrDefaultAsync(query => query.PaymentIntentId == paymentIntentId, cancellationToken);

    public StripeCheckoutSession Update(StripeCheckoutSession stripeCheckoutSession)
    {
        var now = TimeProvider.GetUtcNow();
        stripeCheckoutSession.ModifiedAt = now;
        return DbContext.StripeCheckoutSession.Update(stripeCheckoutSession).Entity;
    }

    public async Task<StripeCheckoutSession?> GetByTransferIdAsync(
        string transferId, CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession.FirstOrDefaultAsync(
            query => query.TransferId == transferId, cancellationToken);

    public async Task<StripeCheckoutSession?> GetByPayoutIdAsync(
        string payoutId, CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession.FirstOrDefaultAsync(
            query => query.PayoutId == payoutId, cancellationToken);

    public async Task<IReadOnlyList<StripeCheckoutSession>> GetDestinationChargeCandidatesAsync(
        string destinationAccountId, IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession
            .Where(query => query.ChargeType == "Destination" &&
                            query.DestinationAccountId == destinationAccountId &&
                            ((query.ChargeId != null && sourceIds.Contains(query.ChargeId)) ||
                             (query.TransferId != null && sourceIds.Contains(query.TransferId))))
            .ToListAsync(cancellationToken);
}
