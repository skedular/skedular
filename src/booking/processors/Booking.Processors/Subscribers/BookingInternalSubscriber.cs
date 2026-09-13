using Api.Shared.Clients.Events.Skedular.BookingInternal.V1;
using Booking.Shared.Services;
using Enterprise.Shared.Messaging;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Type;

namespace Booking.Processors.Subscribers;

public sealed class BookingInternalSubscriber(
    IBookingStripeWebhookProcessor stripeWebhookProcessor,
    IXeroWebhookService xeroWebhookService,
    ILogger<BookingInternalSubscriber> logger) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received BookingInternal event. EventType={EventType}", @event.Metadata.Type);

        switch (@event.Metadata.Type)
        {
            case Type.StripeConnectAccountWebhookEventReceived:
                await stripeWebhookProcessor.ProcessAsync(@event.StripeConnectAccountWebhookEventPayload, cancellationToken);
                break;
            case Type.XeroWebhookEventReceived:
                await xeroWebhookService.ProcessAsync(@event.XeroWebhookEventPayload, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }
}
