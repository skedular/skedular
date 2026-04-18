using Api.Shared.Clients.OpenApi.Skedular.Core.V1;

namespace Core.Domain.IntegrationTests.Api.Rest.CoreControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Core.Api")]
public class GetVersionShould(ICoreCoreClient coreCoreClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await coreCoreClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
