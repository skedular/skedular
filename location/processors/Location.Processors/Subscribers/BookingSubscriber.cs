using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Sanitization;
using Location.Shared.Services;
using Location.Shared.Workflows;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Location.Processors.Subscribers;

public class BookingSubscriber(ITemporalService temporalService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
            case Type.BookingDeleted:
                foreach (var locationId in @event.Data.Booking.InvolvedLocationIds.RemoveInvalidIds().Distinct())
                {
                    await temporalService.StartOrSignalWorkflowRecomputeLocationBookingDerivedStateAsync(
                        new RecomputeLocationBookingDerivedStateInput(locationId),
                        cancellationToken);
                }

                break;
        }

        return EventSubscriberResults.Success;
    }
}
