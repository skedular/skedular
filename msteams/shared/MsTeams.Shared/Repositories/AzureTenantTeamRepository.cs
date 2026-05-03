using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureTenantTeamRepository : IRepository<AzureTenantTeam>
{
    AzureTenantTeam Add(AzureTenantTeam azureTenantTeam);
    AzureTenantTeam Update(AzureTenantTeam azureTenantTeam);
    void RemoveRange(IEnumerable<AzureTenantTeam> tenantMembers);
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

    public void RemoveRange(IEnumerable<AzureTenantTeam> tenantMembers)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.AzureTenantTeam.UpdateRange(tenantMembers.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }
}
