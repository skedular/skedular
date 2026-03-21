using Api.Shared.Clients.OpenApi.Skedular.MsTeams.V1;

namespace MsTeams.Domain.IntegrationTests.Api.Rest.MsTeamsControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Msteams.Api")]
public class GetVersionShould(IMsTeamsClient msTeamsClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await msTeamsClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
