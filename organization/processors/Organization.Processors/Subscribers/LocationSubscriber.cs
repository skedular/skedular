using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using Location = Organization.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    if (string.IsNullOrWhiteSpace(@event.Data.Location.OrganizationId))
                    {
                        break;
                    }

                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization.Id, cancellationToken);
                    ArgumentNullException.ThrowIfNull(existingOrganization);

                    var existingLocation =
                        await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, existingOrganization, cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationUpsertedEventAsync(
                        location,
                        existingLocation,
                        existingOrganization,
                        cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;

            case Type.InvitationToJoinLocationUpserted:
            case Type.InvitationToJoinLocationDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location? existingLocation,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        if (existingLocation is not null && string.IsNullOrWhiteSpace(location.Organization.Id))
        {
            // If location already exist and is now detached from organization, delete it
            _ = repositoryFactory.LocationRepository.Remove(existingLocation);
            await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return;
        }

        if (string.IsNullOrWhiteSpace(location.Organization.Id))
        {
            // Location not attached to any organization, ignoring it
            return;
        }

        _ = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location, existingOrganization))
            : repositoryFactory.LocationRepository.Update(
                mapper.MergeToEntity(location, existingLocation, existingOrganization));
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
