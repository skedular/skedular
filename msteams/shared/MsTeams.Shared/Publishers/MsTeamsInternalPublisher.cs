using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Shared.Publishers;

public interface IMsTeamsInternalPublisher
{
    Task PublishRefreshAzureTenantTeamsAndChannelsAsync(
        IEnumerable<string> azureTenantIds,
        CancellationToken cancellationToken);
}

public class MsTeamsInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IMsTeamsInternalPublisher
{
    public async Task PublishRefreshAzureTenantTeamsAndChannelsAsync(
        IEnumerable<string> azureTenantIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(azureTenantIds.Select(async azureTenantId =>
        {
            var key = new Key { AzureTenantId = azureTenantId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshAzureTenantTeamsAndChannels,
                    context.GetCorrelationId()),
                AzureTenantId = azureTenantId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
