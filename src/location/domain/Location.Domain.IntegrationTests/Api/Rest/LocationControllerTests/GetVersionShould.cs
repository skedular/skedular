using Api.Shared.Clients.OpenApi.Skedular.Location.Core.V1;

namespace Location.Domain.IntegrationTests.Api.Rest.LocationControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class GetVersionShould(ILocationCoreClient locationClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await locationClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
