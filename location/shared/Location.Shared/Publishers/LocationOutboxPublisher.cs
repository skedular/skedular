using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Location.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface ILocationOutboxPublisher
{
    void PublishLocations(IEnumerable<Models.Location> locations, IUnitOfWork unitOfWork);
}

public class LocationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher)
    : ILocationOutboxPublisher
{
    public void PublishLocations(IEnumerable<Models.Location> locations, IUnitOfWork unitOfWork)
    {
        foreach (var location in locations)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }
}
