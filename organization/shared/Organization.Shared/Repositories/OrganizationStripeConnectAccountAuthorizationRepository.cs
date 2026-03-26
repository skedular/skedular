using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripeConnectAccountAuthorizationRepository : IRepository<OrganizationStripeConnectAccountAuthorization>
{
    OrganizationStripeConnectAccountAuthorization Add(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
    void Update(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
    void Remove(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode);
}

public class OrganizationStripeConnectAccountAuthorizationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripeConnectAccountAuthorization>(dbContext, timeProvider),
        IOrganizationStripeConnectAccountAuthorizationRepository
{
    public OrganizationStripeConnectAccountAuthorization Add(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccountAuthorization.Add(stripeConnectAccountRefreshCode).Entity;
    }

    public void Update(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.ModifiedAt = now;
        DbContext.OrganizationStripeConnectAccountAuthorization.Update(stripeConnectAccountRefreshCode);
    }

    public void Remove(OrganizationStripeConnectAccountAuthorization stripeConnectAccountRefreshCode) =>
        DbContext.OrganizationStripeConnectAccountAuthorization.Remove(stripeConnectAccountRefreshCode);
}
