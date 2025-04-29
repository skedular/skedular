using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IStripeConnectAccountRefreshCodeRepository : IRepository<StripeConnectAccountRefreshCode>
{
    Task<StripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    StripeConnectAccountRefreshCode Add(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode);
    StripeConnectAccountRefreshCode Update(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode);
    StripeConnectAccountRefreshCode Remove(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode);
}

public class StripeConnectAccountRefreshCodeRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, StripeConnectAccountRefreshCode>(dbContext, timeProvider),
        IStripeConnectAccountRefreshCodeRepository
{
    public async Task<StripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccountRefreshCode
            .Include(query => query.StripeConnectAccount)
            .FirstOrDefaultAsync(query => query.Code == code, cancellationToken);

    public StripeConnectAccountRefreshCode Add(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.CreatedAt = now;
        return DbContext.StripeConnectAccountRefreshCode.Add(stripeConnectAccountRefreshCode).Entity;
    }

    public StripeConnectAccountRefreshCode Update(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.ModifiedAt = now;
        return DbContext.StripeConnectAccountRefreshCode.Update(stripeConnectAccountRefreshCode).Entity;
    }

    public StripeConnectAccountRefreshCode Remove(StripeConnectAccountRefreshCode stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.DeletedAt = now;
        return DbContext.StripeConnectAccountRefreshCode.Update(stripeConnectAccountRefreshCode).Entity;
    }
}
