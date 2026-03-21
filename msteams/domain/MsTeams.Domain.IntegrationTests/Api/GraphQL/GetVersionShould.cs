using MsTeams.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace MsTeams.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Msteams.Api")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.MsTeamsVersion.ShouldNotBeNull();
    }
}
