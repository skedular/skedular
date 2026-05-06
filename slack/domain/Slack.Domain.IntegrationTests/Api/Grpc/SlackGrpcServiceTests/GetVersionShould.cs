using Api.Shared.Grpc.Skedular.Slack.Core.V1;

namespace Slack.Domain.IntegrationTests.Api.Grpc.SlackGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Slack.Api")]
public class GetVersionShould(SlackService.SlackServiceClient slackServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await slackServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
