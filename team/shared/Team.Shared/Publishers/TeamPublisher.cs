using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Team.Shared.Mappers;
using Team.Shared.Models;
using Event = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Team.Shared.Publishers;

public interface ITeamPublisher
{
    Task PublishTeamAsync(IEnumerable<Models.Team> teams, CancellationToken cancellationToken);

    Task PublishInvitesToJoinTeamNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken);
}

public class TeamPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ITeamPublisher
{
    public async Task PublishTeamAsync(IEnumerable<Models.Team> teams,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(teams.Select(
            team => publisher.PublishAsync(
                new Key { TeamId = team.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        team.IsNotDeleted() ? Type.TeamUpserted : Type.TeamDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data { TeamAfterState = mapper.MapTo(team) }
                },
                cancellationToken)));

    public async Task PublishInvitesToJoinTeamNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        string? inviteeIdToOverride,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(joinInvitations.Select(
            joinInvitation => publisher.PublishAsync(
                new Key { TeamId = joinInvitation.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        joinInvitation.IsNotDeleted()
                            ? Type.InvitationToJoinTeamUpserted
                            : Type.InvitationToJoinTeamDeleted,
                        context.PropertyBag.CorrelationId),
                    Data = new Data
                    {
                        InvitationToJoinTeamAfterState = mapper.MapTo(joinInvitation, inviteeIdToOverride)
                    }
                },
                cancellationToken)));
}
