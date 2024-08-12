using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Shared.Publishers;

public interface IMsTeamsInternalOutboxPublisher
{
    Task PublishRefreshTenantMembersAsync(
        IEnumerable<string> tenantIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class MsTeamsInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IMsTeamsInternalOutboxPublisher
{
    public async Task PublishRefreshTenantMembersAsync(
        IEnumerable<string> tenantIds,
        IUnitOfWork unitOfWork,
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

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));
}
