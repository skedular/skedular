using Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface ILocationInternalPublisher
{
    Task PublishRecordLocationDailyDeskCountAsync(
        IEnumerable<string> locationIds,
        CancellationToken cancellationToken);
}

public class LocationInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ILocationInternalPublisher
{
    public async Task PublishRecordLocationDailyDeskCountAsync(
        IEnumerable<string> locationIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(locationIds.Select(async locationId =>
        {
            var key = new Key { LocationId = locationId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RecordDailyDeskCount,
                    context.GetCorrelationId()),
                LocationId = locationId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
