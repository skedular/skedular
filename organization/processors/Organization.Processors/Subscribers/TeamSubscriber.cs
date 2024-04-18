using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event;
using Team = Organization.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Organization.Processors.Subscribers;

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
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
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
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
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

            case Type.NotificationUpserted:
            case Type.NotificationDeleted:
            default:
                return;
        }
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team? existingTeam,
        CancellationToken cancellationToken)
    {
        if (existingTeam is not null && string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            // If team already exist and is now detached from organization, delete it
            _ = repositoryFactory.TeamRepository.Remove(existingTeam);
            await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return;
        }

        if (string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            // Team not attached to any organization, ignoring it
            return;
        }

        var organization =
            await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _ = existingTeam is null
            ? repositoryFactory.TeamRepository.Add(mapper.MapToEntity(team, organization))
            : repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam,
                organization));

        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
