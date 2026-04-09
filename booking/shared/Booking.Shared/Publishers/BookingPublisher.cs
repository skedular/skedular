using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Booking.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Type;

namespace Booking.Shared.Publishers;

public interface IBookingPublisher
{
    Task PublishBookingsAsync(ICollection<Models.Booking> bookings, CancellationToken cancellationToken);
}

public class BookingPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IBookingPublisher
{
    public async Task PublishBookingsAsync(ICollection<Models.Booking> bookings, CancellationToken cancellationToken) =>
        await Task.WhenAll(bookings.Select(booking => publisher.PublishAsync(
            new Key { BookingId = booking.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    booking.IsDeleted() ? Type.BookingDeleted : Type.BookingUpserted,
                    context.GetCorrelationId()),
                Data = new Data { Booking = mapper.MapTo(booking) }
            },
            cancellationToken)));
}
