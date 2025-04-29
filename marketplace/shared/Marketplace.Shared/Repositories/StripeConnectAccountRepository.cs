using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IStripeConnectAccountRepository : IRepository<StripeConnectAccount>
{
    Task<StripeConnectAccount> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    StripeConnectAccount Update(StripeConnectAccount stripeConnectAccount);
    StripeConnectAccount Remove(StripeConnectAccount stripeConnectAccount);
}

public class StripeConnectAccountRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, StripeConnectAccount>(dbContext, timeProvider), IStripeConnectAccountRepository
{
    public async Task<StripeConnectAccount> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.StripeConnectAccount
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public StripeConnectAccount Update(StripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.ModifiedAt = now;
        return DbContext.StripeConnectAccount.Update(stripeConnectAccount).Entity;
    }

    public StripeConnectAccount Remove(StripeConnectAccount stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.DeletedAt = now;
        return DbContext.StripeConnectAccount.Update(stripeConnectAccount).Entity;
    }
}
