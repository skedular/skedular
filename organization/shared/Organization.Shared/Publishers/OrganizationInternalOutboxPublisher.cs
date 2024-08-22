using Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationInternalOutboxPublisher
{
    Task PublishRefreshAzureTenantMembersAsync(
        IEnumerable<string> azureTenantIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class OrganizationInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IOrganizationInternalOutboxPublisher
{
    public async Task PublishRefreshAzureTenantMembersAsync(
        IEnumerable<string> azureTenantIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(azureTenantIds.Select(async azureTenantId =>
        {
            var key = new Key { AzureTenantId = azureTenantId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshAzureTenantMembers,
                    context.PropertyBag.CorrelationId),
                AzureTenantId = azureTenantId
            };

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));
}
