using Api.Shared.Clients.Events.Skedular.Location.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Kafka;
using Location.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Type;

namespace Location.Shared.Publishers;

public interface ILocationOutboxPublisher
{
    void PublishLocations(IEnumerable<Models.Location> locations, IUnitOfWork unitOfWork);
}

public class LocationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : ILocationOutboxPublisher
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
                        location.IsDeleted() ? Type.LocationDeleted : Type.LocationUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { Location = eventMapper.MapTo(location) }
                },
                unitOfWork);
        }
    }
}
