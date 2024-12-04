using Api.Shared.Clients.Events.UnityHub.Location.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Team.Processors.Mappers;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;
using Event = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event;
using Location = Team.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Type;

namespace Team.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
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
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    if (string.IsNullOrWhiteSpace(@event.Data.LocationAfterState.OrganizationId))
                    {
                        break;
                    }

                    var existingOrganization =
                        location.Organization is null
                            ? null
                            : await repositoryFactory.OrganizationRepository.GetByIdAsync(
                                location.Organization.Id,
                                cancellationToken);
                    ArgumentNullException.ThrowIfNull(existingOrganization);

                    var existingLocation =
                        await repositoryFactory.LocationRepository.UpsertNakedAsync(
                            location.Id,
                            existingOrganization,
                            cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Location event. Event timestamp is older that what is already processed.");

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
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Location event. Event timestamp is older that what is already processed.");

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
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
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
