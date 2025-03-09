using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Kafka.Consume;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class BookingInternalSubscriber(IRepositoryFactory repositoryFactory, IResourceBookingSlotHelper resourceBookingSlotHelper)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.GenerateResourceBookingSlot:
                await HandleGenerateResourceBookingSlotEventAsync(@event.ResourceId, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleGenerateResourceBookingSlotEventAsync(string resourceId, CancellationToken cancellationToken)
    {
        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, false, cancellationToken);
        if (resource is null)
        {
            return;
        }

        var existingResourceBookingSlots = await repositoryFactory.ResourceBookingSlotRepository.GetByResourceIdAsync(
            resourceId,
            resourceBookingSlotHelper.GetStartPeriod(),
            cancellationToken);

        repositoryFactory.ResourceBookingSlotRepository.AddRange(
            resourceBookingSlotHelper
                .CreateAllAvailableSlots(resource)
                .Where(item => existingResourceBookingSlots.All(existingResourceBookingSlot => existingResourceBookingSlot.Start != item.Start))
                .ToList());

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
