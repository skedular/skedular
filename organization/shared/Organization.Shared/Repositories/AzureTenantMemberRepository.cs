using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IAzureTenantMemberRepository : IRepository<AzureTenantMember>
{
    AzureTenantMember Add(AzureTenantMember azureTenantMember);
    AzureTenantMember Update(AzureTenantMember azureTenantMember);
    void RemoveRange(IEnumerable<AzureTenantMember> tenantMembers);

    Task<IReadOnlyList<AzureTenantMember>> GetByTenantIdAsync(
        string tenantId,
        CancellationToken cancellationToken);
}

public class AzureTenantMemberRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, AzureTenantMember>(dbContext, timeProvider), IAzureTenantMemberRepository
{
    public AzureTenantMember Add(AzureTenantMember azureTenantMember)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenantMember.CreatedAt = now;
        return DbContext.AzureTenantMember.Add(azureTenantMember).Entity;
    }

    public AzureTenantMember Update(AzureTenantMember azureTenantMember)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenantMember.ModifiedAt = now;
        return DbContext.AzureTenantMember.Update(azureTenantMember).Entity;
    }

    public void RemoveRange(IEnumerable<AzureTenantMember> tenantMembers)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.AzureTenantMember.UpdateRange(tenantMembers.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public async Task<IReadOnlyList<AzureTenantMember>> GetByTenantIdAsync(
        string tenantId,
        CancellationToken cancellationToken) =>
        await DbContext.AzureTenantMember
            .Where(query => query.AzureTenant.Id == tenantId)
            .AsSingleQuery()
            .Include(query => query.AzureTenant)
            .ToListAsync(cancellationToken);
}
