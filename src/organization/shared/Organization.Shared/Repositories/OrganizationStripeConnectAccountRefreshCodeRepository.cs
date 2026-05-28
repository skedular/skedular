using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRefreshCodeRepository : IRepository<OrganizationStripeConnectAccountRefreshCode>
{
    Task<OrganizationStripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    OrganizationStripeConnectAccountRefreshCode Add(OrganizationStripeConnectAccountRefreshCode stripeConnectAccountRefreshCode);
    OrganizationStripeConnectAccountRefreshCode Remove(OrganizationStripeConnectAccountRefreshCode stripeConnectAccountRefreshCode);
}

public class OrganizationStripeConnectAccountRefreshCodeRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationStripeConnectAccountRefreshCode>(dbContext, timeProvider),
        IOrganizationStripeConnectAccountRefreshCodeRepository
{
    public async Task<OrganizationStripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccountRefreshCode
            .AsSingleQuery()
            .Include(query => query.OrganizationStripeConnectAccount)
            .FirstOrDefaultAsync(query => query.Code == code, cancellationToken);

    public OrganizationStripeConnectAccountRefreshCode Add(OrganizationStripeConnectAccountRefreshCode stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccountRefreshCode.Add(stripeConnectAccountRefreshCode).Entity;
    }

    public OrganizationStripeConnectAccountRefreshCode Remove(OrganizationStripeConnectAccountRefreshCode stripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccountRefreshCode.DeletedAt = now;
        return DbContext.OrganizationStripeConnectAccountRefreshCode.Update(stripeConnectAccountRefreshCode).Entity;
    }
}
