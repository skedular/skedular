using Api.Shared.Clients.OpenApi.Skedular.Core.V1;
using Shouldly;
using Testing.Shared;

namespace Core.Domain.IntegrationTests.Api.Rest.CoreControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Core.Api")]
public class GetVersionShould(ICoreClient coreClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await coreClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
