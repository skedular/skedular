using Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1;
using Marketplace.Domain.IntegrationTests.Clients.GraphQL.V1;
using Shouldly;
using Testing.Shared;

namespace Marketplace.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Marketplace.Api")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.MarketplaceVersion.ShouldNotBeNull();
    }
}
