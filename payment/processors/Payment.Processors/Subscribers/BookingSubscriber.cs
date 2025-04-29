using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Payment.Processors.Mappers;
using Payment.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Booking = Payment.Shared.Database.Entities.Booking;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class BookingSubscriber(ILogger<BookingSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                {
                    var booking = mapper.MapTo(@event);
                    if (!booking.IsPaymentRequired)
                    {
                        await HandleBookingDeletedEventAsync(booking, cancellationToken);
                    }
                    else
                    {
                        var existingBooking = await repositoryFactory.BookingRepository.UpsertNakedAsync(booking.Id, cancellationToken);
                        if (existingBooking.EventRaisedAt > booking.EventRaisedAt)
                        {
                            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

                            return EventSubscriberResults.Success;
                        }

                        await HandleBookingUpsertedEventAsync(booking, existingBooking, cancellationToken);
                    }
                }
                break;

            case Type.BookingDeleted:
                {
                    var booking = mapper.MapTo(@event);
                    await HandleBookingDeletedEventAsync(booking, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleBookingUpsertedEventAsync(Shared.Models.Booking booking, Booking existingBooking, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(booking, existingBooking));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
        if (existingBooking is not null && existingBooking.EventRaisedAt > booking.EventRaisedAt)
        {
            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

            return;
        }

        if (existingBooking is null)
        {
            return;
        }

        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
