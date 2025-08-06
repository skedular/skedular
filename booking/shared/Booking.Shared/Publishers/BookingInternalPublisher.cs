using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Shared.Publishers;

public interface IBookingInternalPublisher
{
    Task PublishStripeConnectAccountWebhookEventReceivedAsync(string id, string payload, CancellationToken cancellationToken);
}

public class BookingInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IBookingInternalPublisher
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
