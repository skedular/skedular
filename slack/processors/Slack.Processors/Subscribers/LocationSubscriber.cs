using Api.Shared.Clients.Events.UnityHub.Location.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Location.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Slack.Shared.Repositories;
using IMapper = Slack.Processors.Mappers.IMapper;
using Location = Slack.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Type;

namespace Slack.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation =
                        await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Location event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    await HandleLocationUpsertedEventAsync(location, existingLocation, cancellationToken);
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

                        return;
                    }

                    if (existingLocation is null)
                    {
                        return;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;

            case Type.NotificationUpserted:
            case Type.NotificationDeleted:
            default:
                return;
        }
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location? existingLocation,
        CancellationToken cancellationToken)
    {
        _ = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location))
            : repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation));

        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
