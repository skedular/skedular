using Enterprise.Shared;
using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IAzureTenantMemberRepository : IRepository<AzureTenantMember>
{
    AzureTenantMember Add(AzureTenantMember azureTenantMember);
    AzureTenantMember Update(AzureTenantMember azureTenantMember);
    void RemoveRange(ICollection<AzureTenantMember> tenantMembers);
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

    public void RemoveRange(ICollection<AzureTenantMember> tenantMembers)
    {
        var now = TimeProvider.GetUtcNow();
        tenantMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.AzureTenantMember.UpdateRange(tenantMembers);
    }
}
