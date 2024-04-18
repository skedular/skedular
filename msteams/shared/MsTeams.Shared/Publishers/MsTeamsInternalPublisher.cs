using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Shared.Publishers;

public interface IMsTeamsInternalPublisher
{
    Task PublishTenantMembersAsync(IEnumerable<string> tenantIds, CancellationToken cancellationToken);
}

public class MsTeamsInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IMsTeamsInternalPublisher
{
    public async Task PublishTenantMembersAsync(
        IEnumerable<string> tenantIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(tenantIds.Select(async tenantId =>
        {
            var key = new Key { TenantId = tenantId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshTenantMembers,
                    context.PropertyBag.CorrelationId),
                TenantId = tenantId
            };
            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
