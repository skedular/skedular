using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeCheckoutSessionRepository : IRepository<StripeCheckoutSession>
{
    Task<StripeCheckoutSession?> GetByStripeCheckoutSessionIdAsync(string stripeCheckoutSessionId, CancellationToken cancellationToken);
    StripeCheckoutSession Add(StripeCheckoutSession stripeCheckoutSession);
    StripeCheckoutSession Update(StripeCheckoutSession stripeCheckoutSession);
}

public class StripeCheckoutSessionRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeCheckoutSession>(dbContext, timeProvider), IStripeCheckoutSessionRepository
{
    public async Task<StripeCheckoutSession?> GetByStripeCheckoutSessionIdAsync(
        string stripeCheckoutSessionId,
        CancellationToken cancellationToken) =>
        await DbContext.StripeCheckoutSession
            .Include(query => query.Booking)
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
