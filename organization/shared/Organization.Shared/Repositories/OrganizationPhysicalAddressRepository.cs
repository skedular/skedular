using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using OrganizationPhysicalAddress = Organization.Shared.Database.Entities.OrganizationPhysicalAddress;

namespace Organization.Shared.Repositories;

public interface IOrganizationPhysicalAddressRepository : IRepository<OrganizationPhysicalAddress>
{
    Task<OrganizationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationPhysicalAddress Add(OrganizationPhysicalAddress stripeConnectAccount);
    OrganizationPhysicalAddress Update(OrganizationPhysicalAddress stripeConnectAccount);
    OrganizationPhysicalAddress Remove(OrganizationPhysicalAddress stripeConnectAccount);
}

internal static class OrganizationPhysicalAddressExtensions
{
    internal static IIncludableQueryable<OrganizationPhysicalAddress, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<OrganizationPhysicalAddress> originalQuery) =>
        originalQuery
            .Include(query => query.Organization);
}

public class OrganizationPhysicalAddressRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationPhysicalAddress>(dbContext, timeProvider), IOrganizationPhysicalAddressRepository
{
    public async Task<OrganizationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationPhysicalAddress
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public OrganizationPhysicalAddress Add(OrganizationPhysicalAddress stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.CreatedAt = now;
        return DbContext.OrganizationPhysicalAddress.Add(stripeConnectAccount).Entity;
    }

    public OrganizationPhysicalAddress Update(OrganizationPhysicalAddress stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.ModifiedAt = now;
        return DbContext.OrganizationPhysicalAddress.Update(stripeConnectAccount).Entity;
    }

    public OrganizationPhysicalAddress Remove(OrganizationPhysicalAddress stripeConnectAccount)
    {
        var now = TimeProvider.GetUtcNow();
        stripeConnectAccount.DeletedAt = now;
        return DbContext.OrganizationPhysicalAddress.Update(stripeConnectAccount).Entity;
    }

    public void RemoveRange(ICollection<OrganizationPhysicalAddress> organizationBankAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        organizationBankAccounts.ForEach(organizationBankAccount => organizationBankAccount.DeletedAt = now);
        DbContext.OrganizationPhysicalAddress.UpdateRange(organizationBankAccounts);
    }
}
