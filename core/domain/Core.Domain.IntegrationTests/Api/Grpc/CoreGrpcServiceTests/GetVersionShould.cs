using Api.Shared.Grpc.Skedular.Core.Core.V1;

namespace Core.Domain.IntegrationTests.Api.Grpc.CoreGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Core.Api")]
public class GetVersionShould(CoreService.CoreServiceClient coreServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await coreServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
