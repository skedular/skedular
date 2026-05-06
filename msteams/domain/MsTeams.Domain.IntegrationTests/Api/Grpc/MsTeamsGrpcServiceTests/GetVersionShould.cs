using Api.Shared.Grpc.Skedular.MsTeams.Core.V1;

namespace MsTeams.Domain.IntegrationTests.Api.Grpc.MsTeamsGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Msteams.Api")]
public class GetVersionShould(MsTeamsService.MsTeamsServiceClient msTeamsServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await msTeamsServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
