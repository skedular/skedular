using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Location.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface ILocationPublisher
{
    Task PublishLocationsAsync(IEnumerable<Models.Location> locations, CancellationToken cancellationToken);
}

public class LocationPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ILocationPublisher
{
    public async Task PublishLocationsAsync(IEnumerable<Models.Location> locations, CancellationToken cancellationToken) =>
        await Task.WhenAll(locations.Select(
            location => publisher.PublishAsync(
                new Key { LocationId = location.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        location.IsNotDeleted() ? Type.LocationUpserted : Type.LocationDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Location = mapper.MapTo(location) }
                },
                cancellationToken)));
}
