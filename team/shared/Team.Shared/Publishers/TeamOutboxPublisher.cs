using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Team.Shared.Mappers;
using Team.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Team.Shared.Publishers;

public interface ITeamOutboxPublisher
{
    Task PublishTeamAsync(
        IEnumerable<Models.Team> teams,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishInvitesToJoinTeamNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class TeamOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : ITeamOutboxPublisher
{
    public async Task PublishTeamAsync(
        IEnumerable<Models.Team> teams,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(teams.Select(team =>
            publisher.PublishAsync(
                new Key { TeamId = team.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        team.IsNotDeleted() ? Type.TeamUpserted : Type.TeamDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { TeamAfterState = mapper.MapTo(team) }
                }, unitOfWork, cancellationToken)));

    public async Task PublishInvitesToJoinTeamNotificationAsync(
        IEnumerable<JoinInvitation> joinInvitations,
        IUnitOfWork unitOfWork,
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
                        context.GetCorrelationId()),
                    Data = new Data { InvitationToJoinTeamAfterState = mapper.MapTo(joinInvitation, null) }
                },
                unitOfWork,
                cancellationToken)));
}
