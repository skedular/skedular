using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Clients.Events.Skedular.Booking.V1.Value;
using Booking.Shared.Mappers;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Booking.Shared.Publishers;

public interface IBookingOutboxPublisher
{
    Task PublishBookingAsync(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
}

public class BookingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IBookingOutboxPublisher
{
    public async Task PublishBookingAsync(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork, CancellationToken cancellationToken) =>
        await Task.WhenAll(bookings.Select(booking =>
            publisher.PublishAsync(
                new Key { BookingId = booking.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        booking.IsNotDeleted() ? Type.BookingUpserted : Type.BookingDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Booking = mapper.MapTo(booking) }
                }, unitOfWork, cancellationToken)));
}
