using Api.Shared.Clients.Events.Skedular.Team.V1;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Kafka;
using Microsoft.Extensions.Logging;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using TeamModel = Team.Shared.Models.Team;

namespace Team.Shared.UnitTests.Publishers.TeamOutboxPublisherTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PublishTeamsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Publish_All_Teams_And_Log_Outcome(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IContext context,
        [Frozen] IKafkaOutboxEventPublisher<Key, Event> publisher,
        [Frozen] ILogger<TeamOutboxPublisher> logger,
        TeamOutboxPublisher sut,
        IUnitOfWork unitOfWork)
    {
        var teams = new List<TeamModel> { new() { Id = "team-1" }, new() { Id = "team-2", DeletedAt = DateTimeOffset.UtcNow } };

        A.CallTo(() => context.GetCorrelationId()).Returns("corr-id");
        A.CallTo(() => eventMapper.MapTo(A<TeamModel>._)).Returns(new Api.Shared.Clients.Events.Skedular.Team.V1.Team());

        sut.PublishTeams(teams, unitOfWork);

        A.CallTo(() => publisher.Publish(A<Key>._, A<Event>._, unitOfWork)).MustHaveHappenedTwiceExactly();
        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .MustHaveHappened();
    }
}
