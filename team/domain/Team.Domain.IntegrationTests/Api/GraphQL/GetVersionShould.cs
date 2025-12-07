using Api.Shared.Clients.OpenApi.Skedular.Team.V1;
using Team.Domain.IntegrationTests.Clients.GraphQL.V1;
using Shouldly;
using Testing.Shared;

namespace Team.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.TeamVersion.ShouldNotBeNull();
    }
}
