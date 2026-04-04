using Api.Shared.Services.Grpc.Skedular.Team.V1;

namespace Team.Domain.IntegrationTests.Api.Grpc.TeamGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class GetVersionShould(TeamService.TeamServiceClient teamServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await teamServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
