using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Booking.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Kafka;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Type;

namespace Booking.Shared.Publishers;

public interface IBookingOutboxPublisher
{
    void PublishBookings(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork);
}

public class BookingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher)
    : IBookingOutboxPublisher
{
    public void PublishBookings(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork)
    {
        foreach (var booking in bookings)
        {
            publisher.Publish(
                new Key
                {
                    BookingId = booking.Id,
                },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        booking.IsDeleted() ? Type.BookingDeleted : Type.BookingUpserted,
                        context.GetCorrelationId()),
                    Data = new Data
                    {
                        Booking = eventMapper.MapTo(booking),
                    },
                },
                unitOfWork);
        }
    }
}
