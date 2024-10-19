using Api.Shared.Clients.Events.UnityHub.Booking.V1.Key;
using Booking.Processors.Mappers;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Event = Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class BookingSubscriber(
    ILogger<BookingSubscriber> logger,
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        if (@event.Metadata.DomainSource == applicationConfiguration.DomainSource)
        {
            // Event raised previously by this domain, ignoring it.
            return EventSubscriberResults.Success;
        }

        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                {
                    var booking = mapper.MapTo(@event);
                    var existingBooking =
                        await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
                    if (existingBooking is not null && existingBooking.ModifiedAt > booking.ModifiedAt)
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
                    if (existingBooking is not null && existingBooking.ModifiedAt > booking.ModifiedAt)
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
        Shared.Database.Entities.Booking? existingBooking,
        CancellationToken cancellationToken)
    {
        var customer =
            await repositoryFactory.CustomerRepository.UpsertNakedAsync(booking.Customer.Id, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var organization = booking.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(booking.Organization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var location = booking.Location is null
            ? null
            : await repositoryFactory.LocationRepository.UpsertNakedAsync(booking.Location.Id, null, cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var desks = new List<Desk>();
        foreach (var desk in booking.Desks)
        {
            desks.Add(await repositoryFactory.DeskRepository.UpsertNakedAsync(desk.Id, null, cancellationToken));
        }

        var team = booking.Team is null
            ? null
            : await repositoryFactory.TeamRepository.UpsertNakedAsync(booking.Team.Id, null, cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _ = existingBooking is null
            ? repositoryFactory.BookingRepository.Add(mapper.MapToEntity(
                booking,
                customer,
                organization,
                location,
                desks,
                team))
            : repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(
                booking,
                existingBooking,
                customer,
                organization,
                location,
                desks,
                team));

        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Shared.Database.Entities.Booking existingBooking,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
