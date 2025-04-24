using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using OrganizationStripeConnectAccount = Marketplace.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRepository : IRepository<OrganizationStripeConnectAccount>
{
    Task<OrganizationStripeConnectAccount> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount organizationStripeConnectAccount);
}

public class OrganizationStripeConnectAccountRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationStripeConnectAccount>(dbContext, timeProvider), IOrganizationStripeConnectAccountRepository
{
    public async Task<OrganizationStripeConnectAccount> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccount
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.ModifiedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(organizationStripeConnectAccount).Entity;
    }

    public OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.DeletedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Update(organizationStripeConnectAccount).Entity;
    }
}
