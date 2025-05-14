using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeConnectAccountAuthorizationRepository : IRepository<StripeConnectAccountAuthorization>
{
    StripeConnectAccountAuthorization Add(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
    void Update(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
    void Remove(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
}

public class StripeConnectAccountAuthorizationRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeConnectAccountAuthorization>(dbContext, timeProvider),
        IStripeConnectAccountAuthorizationRepository
{
    public StripeConnectAccountAuthorization Add(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.CreatedAt = now;
        return DbContext.StripeConnectAccountAuthorization.Add(stripeConnectAccountRefreshCode).Entity;
    }

    public void Update(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.ModifiedAt = now;
        DbContext.StripeConnectAccountAuthorization.Update(stripeConnectAccountRefreshCode);
    }

    public void Remove(StripeConnectAccountAuthorization stripeConnectAccountRefreshCode) =>
        DbContext.StripeConnectAccountAuthorization.Remove(stripeConnectAccountRefreshCode);
}
