using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Type;

namespace Organization.Shared.Publishers;

public interface IOrganizationInternalPublisher
{
    Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken);
}

public class OrganizationInternalPublisher(ApplicationConfiguration applicationConfiguration, IContext context, IKafkaPublisher<Key, Event> publisher)
    : IOrganizationInternalPublisher
{
    public async Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken)
    {
        var key = new Key { StripeConnectAccountWebhookKey = id };
        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.StripeConnectAccountWebhookEventReceived,
                context.GetCorrelationId()),
            StripeConnectAccountWebhookEventPayload = payload
        };

        await publisher.PublishAsync(key, @event, cancellationToken);
    }
}
