using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using OrganizationPhysicalAddress = Organization.Shared.Database.Entities.OrganizationPhysicalAddress;

namespace Organization.Shared.Repositories;

public interface IOrganizationPhysicalAddressRepository : IRepository<OrganizationPhysicalAddress>
{
    Task<OrganizationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationPhysicalAddress Add(OrganizationPhysicalAddress address);
    OrganizationPhysicalAddress Update(OrganizationPhysicalAddress address);
    OrganizationPhysicalAddress Remove(OrganizationPhysicalAddress address);
}

internal static class OrganizationPhysicalAddressExtensions
{
    extension(IQueryable<OrganizationPhysicalAddress> originalQuery)
    {
        internal IIncludableQueryable<OrganizationPhysicalAddress, Database.Entities.Organization> AddDependentObjects() =>
            originalQuery
                .AsSingleQuery()
                .Include(query => query.Organization);
    }
}

public class OrganizationPhysicalAddressRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationPhysicalAddress>(dbContext, timeProvider), IOrganizationPhysicalAddressRepository
{
    public async Task<OrganizationPhysicalAddress?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationPhysicalAddress
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public OrganizationPhysicalAddress Add(OrganizationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        return DbContext.OrganizationPhysicalAddress.Add(address).Entity;
    }

    public OrganizationPhysicalAddress Update(OrganizationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        return DbContext.OrganizationPhysicalAddress.Update(address).Entity;
    }

    public OrganizationPhysicalAddress Remove(OrganizationPhysicalAddress address)
    {
        var now = TimeProvider.GetUtcNow();
        address.DeletedAt = now;
        return DbContext.OrganizationPhysicalAddress.Update(address).Entity;
    }

    public void RemoveRange(ICollection<OrganizationPhysicalAddress> organizationBankAccounts)
    {
        var now = TimeProvider.GetUtcNow();
        organizationBankAccounts.ForEach(organizationBankAccount => organizationBankAccount.DeletedAt = now);
        DbContext.OrganizationPhysicalAddress.UpdateRange(organizationBankAccounts);
    }
}
