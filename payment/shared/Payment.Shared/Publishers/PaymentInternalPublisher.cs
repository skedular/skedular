using Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Key;
using Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Type;

namespace Payment.Shared.Publishers;

public interface IPaymentInternalPublisher
{
    Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken);
}

public class PaymentInternalPublisher(ApplicationConfiguration applicationConfiguration, IContext context, IKafkaPublisher<Key, Event> publisher)
    : IPaymentInternalPublisher
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
            Data = new Data { StripeConnectAccountWebhookEventPayload = payload }
        };

        await publisher.PublishAsync(key, @event, cancellationToken);
    }
}
