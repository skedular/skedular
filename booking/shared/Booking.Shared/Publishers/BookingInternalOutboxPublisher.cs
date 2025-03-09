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
    Task PublishGenerateResourceBookingSlotAsync(IEnumerable<string> resourceIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
}

public class BookingInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IBookingInternalOutboxPublisher
{
    public async Task PublishGenerateResourceBookingSlotAsync(
        IEnumerable<string> resourceIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
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

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));
}
