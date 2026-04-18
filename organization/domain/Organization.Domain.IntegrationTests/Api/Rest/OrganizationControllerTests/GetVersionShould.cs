using Api.Shared.Clients.OpenApi.Skedular.Organization.Core.V1;

namespace Organization.Domain.IntegrationTests.Api.Rest.OrganizationControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class GetVersionShould(IOrganizationCoreClient organizationClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await organizationClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
