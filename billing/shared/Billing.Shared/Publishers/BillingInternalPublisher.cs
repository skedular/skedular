using Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value.Type;

namespace Billing.Shared.Publishers;

public interface IBillingInternalPublisher
{
    Task PublishOrganizationOfferingRequireBillingAsync(
        IEnumerable<string> organizationOfferingIds,
        CancellationToken cancellationToken);
}

public class BillingInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IBillingInternalPublisher
{
    public async Task PublishOrganizationOfferingRequireBillingAsync(
        IEnumerable<string> organizationOfferingIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(organizationOfferingIds.Select(async organizationOfferingId =>
        {
            var key = new Key { OrganizationOfferingId = organizationOfferingId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.GenerateOrganizationOfferingInvoice,
                    context.GetCorrelationId()),
                OrganizationOfferingId = organizationOfferingId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
