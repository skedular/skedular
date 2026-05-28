using Api.Shared.Clients.Events.Skedular.Team.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Microsoft.Extensions.Logging;
using Team.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Team.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Type;

namespace Team.Shared.Publishers;

public interface ITeamPublisher
{
    Task PublishTeamsAsync(IReadOnlyList<Models.Team> teams, CancellationToken cancellationToken);
}

public class TeamPublisher(
    ApplicationConfiguration applicationConfiguration,
    IEventMapper eventMapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher,
    ILogger<TeamPublisher> logger)
    : ITeamPublisher
{
    public async Task PublishTeamsAsync(IReadOnlyList<Models.Team> teams, CancellationToken cancellationToken)
    {
        await Task.WhenAll(teams.Select(team => publisher.PublishAsync(
            new Key { TeamId = team.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    team.IsDeleted() ? Type.TeamDeleted : Type.TeamUpserted,
                    context.GetCorrelationId()),
                Data = new Data { Team = eventMapper.MapTo(team) }
            },
            cancellationToken)));

        logger.LogInformation("Team publisher publish completed for {PublishedCount} events", teams.Count);
    }
}
