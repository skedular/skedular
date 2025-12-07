using Api.Shared.Clients.OpenApi.Skedular.Location.V1;
using Shouldly;
using Testing.Shared;

namespace Location.Domain.IntegrationTests.Api.Rest.LocationControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class GetVersionShould(ILocationClient locationClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await locationClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
