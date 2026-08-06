using Api.Shared.Clients.Events.Skedular.Location.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Location.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Type;

namespace Location.Shared.Publishers;

public interface ILocationPublisher
{
    Task PublishLocationsAsync(IReadOnlyList<Models.Location> locations, CancellationToken cancellationToken);
}

public class LocationPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ILocationPublisher
{
    public async Task PublishLocationsAsync(IReadOnlyList<Models.Location> locations, CancellationToken cancellationToken) =>
        await Task.WhenAll(locations.Select(location => publisher.PublishAsync(
            new Key
            {
                LocationId = location.Id,
            },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    location.IsDeleted() ? Type.LocationDeleted : Type.LocationUpserted,
                    context.GetCorrelationId()),
                Data = new Data
                {
                    Location = eventMapper.MapTo(location),
                },
            },
            cancellationToken)));
}
