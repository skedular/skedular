using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureTenantTeamRepository : IRepository<AzureTenantTeam>
{
    AzureTenantTeam Add(AzureTenantTeam azureTenantTeam);
    AzureTenantTeam Update(AzureTenantTeam azureTenantTeam);
    void RemoveRange(ICollection<AzureTenantTeam> tenantMembers);
}

public class AzureTenantTeamRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureTenantTeam>(dbContext, timeProvider), IAzureTenantTeamRepository
{
    public AzureTenantTeam Add(AzureTenantTeam azureTenantTeam)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenantTeam.CreatedAt = now;
        return DbContext.AzureTenantTeam.Add(azureTenantTeam).Entity;
    }

    public AzureTenantTeam Update(AzureTenantTeam azureTenantTeam)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenantTeam.ModifiedAt = now;
        return DbContext.AzureTenantTeam.Update(azureTenantTeam).Entity;
    }

    public void RemoveRange(ICollection<AzureTenantTeam> tenantMembers)
    {
        var now = TimeProvider.GetUtcNow();
        tenantMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.AzureTenantTeam.UpdateRange(tenantMembers);
    }
}
