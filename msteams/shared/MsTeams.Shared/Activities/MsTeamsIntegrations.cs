using MsTeams.Shared.Mappers;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;
using MsTeams.Shared.Workflows.ReSyncMsTeams;
using Temporalio.Activities;

namespace MsTeams.Shared.Activities;

public class MsTeamsIntegrations(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IGraphService graphService,
    TimeProvider timeProvider,
    ITemporalService temporalService)
{
    [Activity]
    public async Task<bool> ReSyncTeamsAndChannelsAsync(string tenantId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var tenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        var azureTenantTeams = await graphService.GetAzureTenantTeamsAsync(tenantId, cancellationToken);
        var teamsToRemove = tenant.AzureTenantTeams
            .Where(azureTenantMember => azureTenantTeams.All(item => item.Id != azureTenantMember.Id))
            .ToList();
        var updatedTeams = tenant.AzureTenantTeams
            .Where(azureTenantTeam => azureTenantTeams.Any(item => item.Id == azureTenantTeam.Id))
            .Select(azureTenantTeam => repositoryFactory.AzureTenantTeamRepository.Update(
                mapper.MergeToEntity(azureTenantTeams.First(item => item.Id == azureTenantTeam.Id), azureTenantTeam, tenant)))
            .ToList();
        var addedTeams = azureTenantTeams
            .Where(azureTenantTeam => tenant.AzureTenantTeams.All(item => item.Id != azureTenantTeam.Id))
            .Select(item => repositoryFactory.AzureTenantTeamRepository.Add(mapper.MapTo(item, tenant)))
            .ToList();

        repositoryFactory.AzureTenantTeamRepository.RemoveRange(teamsToRemove);
        tenant.AzureTenantTeams = addedTeams.Concat(updatedTeams).Concat(teamsToRemove).ToList();

        foreach (var existingAzureTenantTeam in addedTeams.Concat(updatedTeams))
        {
            var azureTenantTeamChannels = await graphService.GetAzureTenantTeamChannelsAsync(tenantId, existingAzureTenantTeam.Id, cancellationToken);
            var channelsToRemove = existingAzureTenantTeam.AzureTenantTeamChannels
                .Where(azureTenantTeamChannel => azureTenantTeamChannels.All(item => item.Id != azureTenantTeamChannel.Id))
                .ToList();
            var updatedChannels = existingAzureTenantTeam.AzureTenantTeamChannels
                .Where(azureTenantTeamChannel => azureTenantTeamChannels.Any(item => item.Id == azureTenantTeamChannel.Id))
                .Select(azureTenantTeamChannel => repositoryFactory.AzureTenantTeamChannelRepository.Update(
                    mapper.MergeToEntity(
                        azureTenantTeamChannels.First(item => item.Id == azureTenantTeamChannel.Id),
                        azureTenantTeamChannel,
                        existingAzureTenantTeam)))
                .ToList();
            var addedChannels = azureTenantTeamChannels
                .Where(azureTenantTeamChannel => existingAzureTenantTeam.AzureTenantTeamChannels.All(item => item.Id != azureTenantTeamChannel.Id))
                .Select(item => repositoryFactory.AzureTenantTeamChannelRepository.Add(mapper.MapTo(item, existingAzureTenantTeam)))
                .ToList();

            repositoryFactory.AzureTenantTeamChannelRepository.RemoveRange(channelsToRemove);
            existingAzureTenantTeam.AzureTenantTeamChannels = addedChannels.Concat(updatedChannels).Concat(channelsToRemove).ToList();
        }

        repositoryFactory.AzureTenantRepository.Update(tenant);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task ExecuteNextReSyncMsTeamsWorkflowAsync(string tenantId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await temporalService.StartWorkflowReSyncMsTeamsAsync(
            new ReSyncMsTeamsInput(tenantId, timeProvider.GetUtcNow().AddDays(1)),
            cancellationToken);
    }
}
