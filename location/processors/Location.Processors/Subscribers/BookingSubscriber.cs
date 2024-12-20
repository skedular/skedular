using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Location.Processors.Mappers;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Booking = Location.Shared.Database.Entities.Booking;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Location.Processors.Subscribers;

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
        if (existingBooking is not null && string.IsNullOrWhiteSpace(booking.Location.Id))
        {
            // If booking already exist and is now detached from location, delete it
            _ = repositoryFactory.BookingRepository.Remove(existingBooking);
            await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return;
        }

        if (string.IsNullOrWhiteSpace(booking.Location.Id))
        {
            // Booking not attached to any location, ignoring it
            return;
        }

        var desks = new List<Desk>();
        if (booking.Desks.Count != 0)
        {
            var deskIds = booking.Desks.Select(item => item.Id).ToList();
            desks =
                await repositoryFactory.DeskRepository.Query(new Specification<Desk>
                {
                    Criteria = query => deskIds.Contains(query.Id)
                }).ToListAsync(cancellationToken);
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(booking.Location.Id, cancellationToken);
        ArgumentNullException.ThrowIfNull(location);

        _ = existingBooking is null
            ? repositoryFactory.BookingRepository.Add(mapper.MapToEntity(booking, location, desks))
            : repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(
                booking,
                existingBooking,
                location,
                desks));

        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Booking existingBooking, CancellationToken cancellationToken)
    {
        existingBooking.Desks = [];

        existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
