using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using MsTeams.Processors.Mappers;
using MsTeams.Processors.Services;
using MsTeams.Shared.Repositories;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Processors.Subscribers;

public class MsTeamsInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    IGraphService graphService)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RefreshAzureTenantTeamsAndChannels:
                await HandleRefreshAzureTenantTeamsAndChannelsEventAsync(@event.AzureTenantId, cancellationToken);
                break;

            default:
                return;
        }
    }

    private async Task HandleRefreshAzureTenantTeamsAndChannelsEventAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var teams = await graphService.GetTeamsAsync(tenantId, cancellationToken);
        foreach (var team in teams)
        {
            var channels = await graphService.GetTeamChannelsAsync(tenantId, team.Id!, cancellationToken);
        }
    }
}
