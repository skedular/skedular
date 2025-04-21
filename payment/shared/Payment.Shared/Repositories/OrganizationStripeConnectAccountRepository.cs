using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRepository : IRepository<OrganizationStripeConnectAccount>
{
    OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    OrganizationStripeConnectAccount Update(OrganizationStripeConnectAccount organizationStripeConnectAccount);
    OrganizationStripeConnectAccount Remove(OrganizationStripeConnectAccount organizationStripeConnectAccount);
}

public class OrganizationStripeConnectAccountRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationStripeConnectAccount>(dbContext, timeProvider), IOrganizationStripeConnectAccountRepository
{
    public OrganizationStripeConnectAccount Add(OrganizationStripeConnectAccount organizationStripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccount.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccount.Add(organizationStripeConnectAccount).Entity;
    }

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
