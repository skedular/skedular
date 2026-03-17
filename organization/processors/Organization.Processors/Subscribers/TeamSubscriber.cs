using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Services;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event;
using Team = Organization.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class TeamSubscriber(ILogger<TeamSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    if (string.IsNullOrWhiteSpace(team.Organization.Id))
                    {
                        await HandleTeamDeletedEventAsync(team, cancellationToken);
                    }
                    else
                    {
                        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                               team.Organization.Id,
                                               null,
                                               cancellationToken) ??
                                           throw new OrganizationNotFound();
                        var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, organization, cancellationToken);
                        if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                        {
                            logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                            return EventSubscriberResults.Success;
                        }

                        await HandleTeamUpsertedEventAsync(team, existingTeam, organization, cancellationToken);
                    }
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
                    await HandleTeamDeletedEventAsync(team, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team existingTeam,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam, existingOrganization));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Shared.Models.Team team, CancellationToken cancellationToken)
    {
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
        if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
        {
            logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

            return;
        }

        if (existingTeam is null)
        {
            return;
        }

        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
