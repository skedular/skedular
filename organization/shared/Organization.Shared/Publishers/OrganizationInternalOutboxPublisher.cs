using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationInternalOutboxPublisher
{
    Task PublishRefreshAzureTenantMembersAsync(IEnumerable<string> azureTenantIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
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
        CancellationToken cancellationToken)
    {
        foreach (var azureTenantId in azureTenantIds)
        {
            await publisher.PublishAsync(
                new Key { AzureTenantId = azureTenantId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.RefreshAzureTenantMembers,
                        context.GetCorrelationId()),
                    AzureTenantId = azureTenantId
                },
                unitOfWork,
                cancellationToken);
        }
    }
}
