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
    void PublishRefreshAzureTenantMembers(IEnumerable<string> azureTenantIds, IUnitOfWork unitOfWork);
}

public class OrganizationInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : IOrganizationInternalOutboxPublisher
{
    public void PublishRefreshAzureTenantMembers(IEnumerable<string> azureTenantIds, IUnitOfWork unitOfWork)
    {
        foreach (var azureTenantId in azureTenantIds)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }
}
