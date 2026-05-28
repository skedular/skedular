using Api.Shared.Clients.OpenApi.Skedular.Marketplace.Core.V1;

namespace Marketplace.Domain.IntegrationTests.Api.Rest.MarketplaceControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Marketplace.Api")]
public class GetVersionShould(IMarketplaceCoreClient marketplaceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await marketplaceClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
