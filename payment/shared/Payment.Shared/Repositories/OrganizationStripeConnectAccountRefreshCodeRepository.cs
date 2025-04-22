using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;
using OrganizationStripeConnectAccountRefreshCode = Payment.Shared.Database.Entities.OrganizationStripeConnectAccountRefreshCode;

namespace Payment.Shared.Repositories;

public interface IOrganizationStripeConnectAccountRefreshCodeRepository : IRepository<OrganizationStripeConnectAccountRefreshCode>
{
    Task<OrganizationStripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    OrganizationStripeConnectAccountRefreshCode Add(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode);
    OrganizationStripeConnectAccountRefreshCode Update(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode);
    OrganizationStripeConnectAccountRefreshCode Remove(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode);
}

public class OrganizationStripeConnectAccountRefreshCodeRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, OrganizationStripeConnectAccountRefreshCode>(dbContext, timeProvider),
        IOrganizationStripeConnectAccountRefreshCodeRepository
{
    public async Task<OrganizationStripeConnectAccountRefreshCode?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        await DbContext.OrganizationStripeConnectAccountRefreshCode
            .Include(query => query.OrganizationStripeConnectAccount)
            .ThenInclude(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Code == code, cancellationToken);

    public OrganizationStripeConnectAccountRefreshCode Add(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccountRefreshCode.CreatedAt = now;
        return DbContext.OrganizationStripeConnectAccountRefreshCode.Add(organizationStripeConnectAccountRefreshCode).Entity;
    }

    public OrganizationStripeConnectAccountRefreshCode Update(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccountRefreshCode.ModifiedAt = now;
        return DbContext.OrganizationStripeConnectAccountRefreshCode.Update(organizationStripeConnectAccountRefreshCode).Entity;
    }

    public OrganizationStripeConnectAccountRefreshCode Remove(OrganizationStripeConnectAccountRefreshCode organizationStripeConnectAccountRefreshCode)
    {
        var now = TimeProvider.GetUtcNow();
        organizationStripeConnectAccountRefreshCode.DeletedAt = now;
        return DbContext.OrganizationStripeConnectAccountRefreshCode.Update(organizationStripeConnectAccountRefreshCode).Entity;
    }
}
