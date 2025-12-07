using Api.Shared.Clients.OpenApi.Skedular.Team.V1;
using Shouldly;
using Testing.Shared;

namespace Team.Domain.IntegrationTests.Api.Rest.TeamControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class GetVersionShould(ITeamClient teamClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await teamClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
