using Api.Shared.Clients.Events.Skedular.Team.V1;
using Enterprise.Shared.Kafka.Consume;
using MsTeams.Processors.Mappers;
using MsTeams.Shared.Repositories;
using Team = MsTeams.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Type;

namespace MsTeams.Processors.Subscribers;

public class TeamSubscriber(ILogger<TeamSubscriber> logger, IEventMapper eventMapper, IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = eventMapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, cancellationToken);
                    if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = eventMapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingTeam is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamDeletedEventAsync(existingTeam, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleTeamUpsertedEventAsync(Shared.Models.Team team, Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Update(eventMapper.MergeToEntity(team, existingTeam));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
