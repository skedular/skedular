using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface ILocationPublisher
{
    Task PublishLocationAsync(IEnumerable<Models.Location> locations, CancellationToken cancellationToken);

    Task PublishInvitesToJoinLocationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken);
}

public class LocationPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ILocationPublisher
{
    public async Task PublishLocationAsync(IEnumerable<Models.Location> locations,
        CancellationToken cancellationToken) =>
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

    public async Task PublishInvitesToJoinLocationNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(joinInvitations.Select(
            joinInvitation => publisher.PublishAsync(
                new Key { LocationId = joinInvitation.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        joinInvitation.IsNotDeleted()
                            ? Type.InvitationToJoinLocationUpserted
                            : Type.InvitationToJoinLocationDeleted,
                        context.GetCorrelationId()),
                    Data = new Data
                    {
                        InvitationToJoinLocation = mapper.MapTo(joinInvitation, inviteeIdToOverride)
                    }
                },
                cancellationToken)));
}
