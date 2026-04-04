using Api.Shared.Services.Grpc.Skedular.Location.V1;

namespace Location.Domain.IntegrationTests.Api.Grpc.LocationGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class GetVersionShould(LocationService.LocationServiceClient locationServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await locationServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
