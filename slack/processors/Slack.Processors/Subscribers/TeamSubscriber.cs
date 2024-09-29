using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Slack.Shared.Repositories;
using IMapper = Slack.Processors.Mappers.IMapper;
using Team = Slack.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Slack.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam =
                        await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Team event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam =
                        await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Team event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    if (existingTeam is null)
                    {
                        return;
                    }

                    await HandleTeamDeletedEventAsync(existingTeam, cancellationToken);
                }
                break;

            case Type.InvitationToJoinTeamUpserted:
            case Type.InvitationToJoinTeamDeleted:
            default:
                return;
        }
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team? existingTeam,
        CancellationToken cancellationToken)
    {
        _ = existingTeam is null
            ? repositoryFactory.TeamRepository.Add(mapper.MapToEntity(team))
            : repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam));

        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
