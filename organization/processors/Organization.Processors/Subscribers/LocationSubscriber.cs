using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Services;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using Location = Organization.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class LocationSubscriber(ILogger<LocationSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    if (string.IsNullOrWhiteSpace(location.Organization.Id))
                    {
                        await HandleLocationDeletedEventAsync(location, cancellationToken);
                    }
                    else
                    {
                        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                               location.Organization.Id,
                                               null,
                                               cancellationToken) ??
                                           throw new OrganizationNotFound();
                        var existingLocation =
                            await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, organization, cancellationToken);
                        if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                        {
                            logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                            return EventSubscriberResults.Success;
                        }

                        await HandleLocationUpsertedEventAsync(location, existingLocation, organization, cancellationToken);
                    }
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    await HandleLocationDeletedEventAsync(location, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation, existingOrganization));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Shared.Models.Location location, CancellationToken cancellationToken)
    {
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
        if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
        {
            logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

            return;
        }

        if (existingLocation is null)
        {
            return;
        }

        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
