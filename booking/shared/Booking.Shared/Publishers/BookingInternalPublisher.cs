using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Shared.Publishers;

public interface IBookingInternalPublisher
{
    Task PublishGenerateResourceBookingSlotAsync(IEnumerable<string> resourceIds, CancellationToken cancellationToken);
    Task PublishPurgeExpiredBookingAsync(IEnumerable<string> bookingIds, CancellationToken cancellationToken);
}

public class BookingInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IBookingInternalPublisher
{
    public async Task PublishGenerateResourceBookingSlotAsync(IEnumerable<string> resourceIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(resourceIds.Select(async resourceId =>
        {
            var key = new Key { ResourceId = resourceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.GenerateResourceBookingSlot,
                    context.GetCorrelationId()),
                ResourceId = resourceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishPurgeExpiredBookingAsync(IEnumerable<string> bookingIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(bookingIds.Select(async bookingId =>
        {
            var key = new Key { BookingId = bookingId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.PurgeExpiredBooking,
                    context.GetCorrelationId()),
                BookingId = bookingId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
