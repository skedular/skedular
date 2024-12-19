using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Team.Processors.Mappers;
using Team.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Booking = Team.Shared.Database.Entities.Booking;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Team.Processors.Subscribers;

public class BookingSubscriber(
    ILogger<BookingSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                {
                    var booking = mapper.MapTo(@event);
                    var existingBooking =
                        await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
                    if (existingBooking is not null && existingBooking.EventRaisedAt > booking.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Booking event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleBookingUpsertedEventAsync(booking, existingBooking, cancellationToken);
                }
                break;

            case Type.BookingDeleted:
                {
                    var booking = mapper.MapTo(@event);
                    var existingBooking =
                        await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
                    if (existingBooking is not null && existingBooking.EventRaisedAt > booking.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Booking event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingBooking is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleBookingDeletedEventAsync(existingBooking, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleBookingUpsertedEventAsync(
        Shared.Models.Booking booking,
        Booking? existingBooking,
        CancellationToken cancellationToken)
    {
        if (existingBooking is not null && string.IsNullOrWhiteSpace(booking.Team.Id))
        {
            // If booking already exist and is now detached from team, delete it
            _ = repositoryFactory.BookingRepository.Remove(existingBooking);
            await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return;
        }

        if (string.IsNullOrWhiteSpace(booking.Team.Id))
        {
            // Booking not attached to any team, ignoring it
            return;
        }

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(booking.Team.Id, cancellationToken);
        ArgumentNullException.ThrowIfNull(team);

        _ = existingBooking is null
            ? repositoryFactory.BookingRepository.Add(mapper.MapToEntity(booking, team))
            : repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(booking, existingBooking,
                team));

        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Booking existingBooking, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
