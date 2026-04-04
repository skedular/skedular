using Api.Shared.Services.Grpc.Skedular.Marketplace.V1;

namespace Marketplace.Domain.IntegrationTests.Api.Grpc.MarketplaceGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Marketplace.Api")]
public class GetVersionShould(MarketplaceService.MarketplaceServiceClient marketplaceServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await marketplaceServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
