using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Shared.Publishers;

public interface IBookingInternalOutboxPublisher
{
    void PublishGenerateResourceBookingSlot(IEnumerable<string> resourceIds, IUnitOfWork unitOfWork);
    void PublishPurgeExpiredBooking(IEnumerable<string> bookingIds, IUnitOfWork unitOfWork);
}

public class BookingInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IBookingInternalOutboxPublisher
{
    public void PublishGenerateResourceBookingSlot(IEnumerable<string> resourceIds, IUnitOfWork unitOfWork)
    {
        foreach (var resourceId in resourceIds)
        {
            publisher.Publish(
                new Key { ResourceId = resourceId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.GenerateResourceBookingSlot,
                        context.GetCorrelationId()),
                    ResourceId = resourceId
                },
                unitOfWork);
        }
    }

    public void PublishPurgeExpiredBooking(IEnumerable<string> bookingIds, IUnitOfWork unitOfWork)
    {
        foreach (var bookingId in bookingIds)
        {
            publisher.Publish(
                new Key { BookingId = bookingId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.PurgeExpiredBooking,
                        context.GetCorrelationId()),
                    BookingId = bookingId
                },
                unitOfWork);
        }
    }
}
