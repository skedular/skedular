using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Sanitization;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Type;

namespace Organization.Processors.Subscribers;

public class BookingSubscriber(
    ITemporalService temporalService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
            case Type.BookingDeleted:
                foreach (var organizationId in @event.Data.Booking.InvolvedOrganizationIds.RemoveInvalidIds().Distinct())
                {
                    await temporalService.StartOrSignalWorkflowRecomputeOrganizationBookingDerivedStateAsync(
                        new RecomputeOrganizationBookingDerivedStateInput(organizationId),
                        cancellationToken);
                }

                break;
        }

        return EventSubscriberResults.Success;
    }
}
