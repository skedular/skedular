using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event;
using Team = Organization.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    if (string.IsNullOrWhiteSpace(@event.Data.Team.OrganizationId))
                    {
                        break;
                    }

                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(team.Organization.Id, cancellationToken);
                    ArgumentNullException.ThrowIfNull(existingOrganization);

                    var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(
                        team.Id,
                        existingOrganization,
                        cancellationToken);
                    if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, existingOrganization, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
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

            case Type.InvitationToJoinTeamUpserted:
            case Type.InvitationToJoinTeamDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team? existingTeam,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        if (existingTeam is not null && string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            // If team already exist and is now detached from organization, delete it
            _ = repositoryFactory.TeamRepository.Remove(existingTeam);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            return;
        }

        if (string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            // Team not attached to any organization, ignoring it
            return;
        }

        _ = existingTeam is null
            ? repositoryFactory.TeamRepository.Add(mapper.MapToEntity(team, existingOrganization))
            : repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam,
                existingOrganization));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
