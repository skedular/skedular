using Enterprise.Shared;
using Enterprise.Shared.Database;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ITenantMemberRepository : IRepository<TenantMember>
{
    TenantMember Add(TenantMember tenantMember);
    TenantMember Update(TenantMember tenantMember);
    void RemoveRange(ICollection<TenantMember> tenantMembers);
}

public class TenantMemberRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, TenantMember>(dbContext), ITenantMemberRepository
{
    public TenantMember Add(TenantMember tenantMember)
    {
        var now = timeProvider.GetUtcNow();
        tenantMember.CreatedAt = now;
        return DbContext.TenantMember.Add(tenantMember).Entity;
    }

    public TenantMember Update(TenantMember tenantMember)
    {
        var now = timeProvider.GetUtcNow();
        tenantMember.ModifiedAt = now;
        return DbContext.TenantMember.Update(tenantMember).Entity;
    }

    public void RemoveRange(ICollection<TenantMember> tenantMembers)
    {
        var now = timeProvider.GetUtcNow();
        tenantMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.TenantMember.RemoveRange(tenantMembers);
    }
}
