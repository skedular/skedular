using Api.Shared.Clients.Events.Skedular.Location.V1;
using Enterprise.Shared.Kafka.Consume;
using Slack.Shared.Repositories;
using IMapper = Slack.Processors.Mappers.IMapper;
using Location = Slack.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Type;

namespace Slack.Processors.Subscribers;

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
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationUpsertedEventAsync(location, existingLocation, cancellationToken);
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
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location existingLocation,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
