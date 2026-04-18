using Api.Shared.Clients.OpenApi.Skedular.Slack.Core.V1;

namespace Slack.Domain.IntegrationTests.Api.Rest.SlackControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Slack.Api")]
public class GetVersionShould(ISlackCoreClient slackClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await slackClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
