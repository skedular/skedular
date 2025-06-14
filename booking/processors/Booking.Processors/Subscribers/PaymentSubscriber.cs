using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Api.Shared.Services;
using Booking.Processors.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Kafka.Consume;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class PaymentSubscriber(ILogger<PaymentSubscriber> logger, IRepositoryFactory repositoryFactory, IMapper mapper) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingCheckoutSessionUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.BookingCheckoutSession.BookingId);

                    var bookingCheckoutSession = mapper.MapTo(@event);
                    var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingCheckoutSession.Booking.Id, cancellationToken);
                    if (booking is null)
                    {
                        throw new BookingNotFound();
                    }

                    var existingBookingCheckoutSession = await repositoryFactory.BookingCheckoutSessionRepository.UpsertNakedAsync(
                        bookingCheckoutSession.Id,
                        booking,
                        cancellationToken);
                    if (existingBookingCheckoutSession.EventRaisedAt > bookingCheckoutSession.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Payment - Booking Checkout Session event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleBookingCheckoutSessionUpsertedEventAsync(
                        bookingCheckoutSession,
                        existingBookingCheckoutSession,
                        booking,
                        cancellationToken);
                }
                break;

            case Type.BookingCheckoutSessionDeleted:
                {
                    var bookingCheckoutSession = mapper.MapTo(@event);
                    var existingBookingCheckoutSession = await repositoryFactory.BookingCheckoutSessionRepository.GetByIdAsync(
                        bookingCheckoutSession.Id,
                        cancellationToken);
                    if (existingBookingCheckoutSession is not null &&
                        existingBookingCheckoutSession.EventRaisedAt > bookingCheckoutSession.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Payment - Booking Checkout Session event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingBookingCheckoutSession is null)
                    {
                        return EventSubscriberResults.Success;
                    }


                    await HandleBookingCheckoutSessionDeletedEventAsync(existingBookingCheckoutSession, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleBookingCheckoutSessionUpsertedEventAsync(
        BookingCheckoutSession bookingCheckoutSession,
        Shared.Database.Entities.BookingCheckoutSession existingBookingCheckoutSession,
        Shared.Database.Entities.Booking booking,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.BookingCheckoutSessionRepository.Update(
            mapper.MergeToEntity(bookingCheckoutSession, existingBookingCheckoutSession, booking));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingCheckoutSessionDeletedEventAsync(
        Shared.Database.Entities.BookingCheckoutSession bookingCheckoutSession,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.BookingCheckoutSessionRepository.Remove(bookingCheckoutSession);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
