using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Logging;
using Team.Api.Controllers;
using Team.Api.Services;

namespace Team.Api.UnitTests.Controllers.TeamControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetVersionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_information_for_get_version(
        [Frozen] IVersionService versionService,
        [Frozen] TeamConfiguration teamConfiguration,
        [Frozen] IWorkaroundService workaroundService,
        [Frozen] ITopicEventSender topicEventSender,
        [Frozen] ILogger<TeamController> logger)
    {
        A.CallTo(() => versionService.GetVersion()).Returns(new Version(1, 2, 3, 4));

        var sut = new TeamController(versionService, teamConfiguration, workaroundService, topicEventSender, logger);

        await sut.GetVersion(CancellationToken.None);

        var logCalls = Fake.GetCalls(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .ToList();

        logCalls.Count(call => Equals(call.Arguments[0], LogLevel.Information)).ShouldBe(2);
    }
}
