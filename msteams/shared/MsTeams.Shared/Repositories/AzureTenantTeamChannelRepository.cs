using Enterprise.Shared;
using Enterprise.Shared.Database;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureTenantTeamChannelRepository : IRepository<AzureTenantTeamChannel>
{
    AzureTenantTeamChannel Add(AzureTenantTeamChannel azureTenantTeamChannel);
    AzureTenantTeamChannel Update(AzureTenantTeamChannel azureTenantTeamChannel);
    void RemoveRange(ICollection<AzureTenantTeamChannel> tenantMembers);
}

public class AzureTenantTeamChannelRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureTenantTeamChannel>(dbContext), IAzureTenantTeamChannelRepository
{
    public AzureTenantTeamChannel Add(AzureTenantTeamChannel azureTenantTeamChannel)
    {
        var now = timeProvider.GetUtcNow();
        azureTenantTeamChannel.CreatedAt = now;
        return DbContext.AzureTenantTeamChannel.Add(azureTenantTeamChannel).Entity;
    }

    public AzureTenantTeamChannel Update(AzureTenantTeamChannel azureTenantTeamChannel)
    {
        var now = timeProvider.GetUtcNow();
        azureTenantTeamChannel.ModifiedAt = now;
        return DbContext.AzureTenantTeamChannel.Update(azureTenantTeamChannel).Entity;
    }

    public void RemoveRange(ICollection<AzureTenantTeamChannel> tenantMembers)
    {
        var now = timeProvider.GetUtcNow();
        tenantMembers.ForEach(teamMember => teamMember.DeletedAt = now);
        DbContext.AzureTenantTeamChannel.UpdateRange(tenantMembers);
    }
}
