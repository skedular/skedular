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
    void PublishTeams(IEnumerable<Models.Team> teams, IUnitOfWork unitOfWork);
    void PublishInvitesToJoinTeamNotification(IEnumerable<JoinInvitation> joinInvitations, IUnitOfWork unitOfWork);
}

public class TeamOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : ITeamOutboxPublisher
{
    public void PublishTeams(IEnumerable<Models.Team> teams, IUnitOfWork unitOfWork)
    {
        foreach (var team in teams)
        {
            publisher.Publish(
                new Key { TeamId = team.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        team.IsNotDeleted() ? Type.TeamUpserted : Type.TeamDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Team = mapper.MapTo(team) }
                },
                unitOfWork);
        }
    }

    public void PublishInvitesToJoinTeamNotification(IEnumerable<JoinInvitation> joinInvitations, IUnitOfWork unitOfWork)
    {
        foreach (var joinInvitation in joinInvitations)
        {
            publisher.Publish(
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
                    Data = new Data { InvitationToJoinTeam = mapper.MapTo(joinInvitation, null) }
                },
                unitOfWork);
        }
    }
}
