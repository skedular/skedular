using Api.Shared.Services.Grpc.Skedular.Organization.V1;

namespace Organization.Domain.IntegrationTests.Api.Grpc.OrganizationGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class GetVersionShould(OrganizationService.OrganizationServiceClient organizationServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await organizationServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
