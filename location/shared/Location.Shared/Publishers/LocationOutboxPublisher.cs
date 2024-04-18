using Api.Shared.Clients.Events.UnityHub.Location.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Location.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Event = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface ILocationOutboxPublisher
{
    Task PublishLocationAsync(
        IEnumerable<Models.Location> locations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishInvitesToJoinLocationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class LocationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : ILocationOutboxPublisher
{
    public async Task PublishLocationAsync(
        IEnumerable<Models.Location> locations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(locations.Select(location =>
            publisher.PublishAsync(
                new Key { LocationId = location.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        location.IsNotDeleted() ? Type.LocationUpserted : Type.LocationDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { LocationAfterState = mapper.MapTo(location) }
                }, unitOfWork, cancellationToken)));

    public async Task PublishInvitesToJoinLocationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(joinInvitations.Select(
            joinInvitation => publisher.PublishAsync(
                new Key { LocationId = joinInvitation.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        joinInvitation.IsNotDeleted() ? Type.NotificationUpserted : Type.NotificationDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { NotificationAfterState = mapper.MapTo(joinInvitation, null) }
                },
                unitOfWork,
                cancellationToken)));
}
