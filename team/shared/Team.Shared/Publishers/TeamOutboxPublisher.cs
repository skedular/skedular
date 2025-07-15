using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Team.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Team.Shared.Publishers;

public interface ITeamOutboxPublisher
{
    void PublishTeams(IEnumerable<Models.Team> teams, IUnitOfWork unitOfWork);
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
                        team.IsDeleted() ? Type.TeamDeleted : Type.TeamUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { Team = mapper.MapTo(team) }
                },
                unitOfWork);
        }
    }
}
