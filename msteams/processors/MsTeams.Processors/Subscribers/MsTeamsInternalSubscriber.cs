using Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using MsTeams.Processors.Mappers;
using MsTeams.Processors.Services;
using MsTeams.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Processors.Subscribers;

public class MsTeamsInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IMapper mapper,
    IGraphService graphService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RefreshAzureTenantTeamsAndChannels:
                await HandleRefreshAzureTenantTeamsAndChannelsEventAsync(@event.AzureTenantId, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleRefreshAzureTenantTeamsAndChannelsEventAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var existingTenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (existingTenant is null)
        {
            return;
        }

        var azureTenantTeams = await graphService.GetAzureTenantTeamsAsync(tenantId, cancellationToken);
        var teamsToRemove = existingTenant.AzureTenantTeams
            .Where(azureTenantMember => azureTenantTeams.All(item => item.Id != azureTenantMember.Id))
            .ToList();
        var updatedTeams = existingTenant.AzureTenantTeams
            .Where(azureTenantTeam => azureTenantTeams.Any(item => item.Id == azureTenantTeam.Id))
            .Select(azureTenantTeam => repositoryFactory.AzureTenantTeamRepository.Update(
                mapper.MergeToEntity(
                    azureTenantTeams.First(item => item.Id == azureTenantTeam.Id),
                    azureTenantTeam,
                    existingTenant)))
            .ToList();
        var addedTeams = azureTenantTeams
            .Where(azureTenantTeam => existingTenant.AzureTenantTeams.All(item => item.Id != azureTenantTeam.Id))
            .Select(item => repositoryFactory.AzureTenantTeamRepository.Add(mapper.MapTo(item, existingTenant)))
            .ToList();

        repositoryFactory.AzureTenantTeamRepository.RemoveRange(teamsToRemove);
        existingTenant.AzureTenantTeams = addedTeams.Concat(updatedTeams).Concat(teamsToRemove).ToList();

        foreach (var existingAzureTenantTeam in addedTeams.Concat(updatedTeams))
        {
            var azureTenantTeamChannels =
                await graphService.GetAzureTenantTeamChannelsAsync(tenantId, existingAzureTenantTeam.Id,
                    cancellationToken);
            var channelsToRemove = existingAzureTenantTeam.AzureTenantTeamChannels
                .Where(azureTenantTeamChannel =>
                    azureTenantTeamChannels.All(item => item.Id != azureTenantTeamChannel.Id))
                .ToList();
            var updatedChannels = existingAzureTenantTeam.AzureTenantTeamChannels
                .Where(azureTenantTeamChannel =>
                    azureTenantTeamChannels.Any(item => item.Id == azureTenantTeamChannel.Id))
                .Select(azureTenantTeamChannel => repositoryFactory.AzureTenantTeamChannelRepository.Update(
                    mapper.MergeToEntity(
                        azureTenantTeamChannels.First(item => item.Id == azureTenantTeamChannel.Id),
                        azureTenantTeamChannel,
                        existingAzureTenantTeam)))
                .ToList();
            var addedChannels = azureTenantTeamChannels
                .Where(azureTenantTeamChannel =>
                    existingAzureTenantTeam.AzureTenantTeamChannels.All(item => item.Id != azureTenantTeamChannel.Id))
                .Select(item =>
                    repositoryFactory.AzureTenantTeamChannelRepository.Add(mapper.MapTo(item, existingAzureTenantTeam)))
                .ToList();

            repositoryFactory.AzureTenantTeamChannelRepository.RemoveRange(channelsToRemove);
            existingAzureTenantTeam.AzureTenantTeamChannels =
                addedChannels.Concat(updatedChannels).Concat(channelsToRemove).ToList();
        }

        existingTenant.TeamsAndChannelsLastRefreshedAt = timeProvider.GetUtcNow();
        repositoryFactory.AzureTenantRepository.Update(existingTenant);

        await repositoryFactory.AzureTenantTeamChannelRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.AzureTenantTeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.AzureTenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
