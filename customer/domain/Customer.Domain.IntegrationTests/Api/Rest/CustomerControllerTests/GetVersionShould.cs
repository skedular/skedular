using Api.Shared.Clients.OpenApi.Skedular.Customer.Core.V1;

namespace Customer.Domain.IntegrationTests.Api.Rest.CustomerControllerTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class GetVersionShould(ICustomerCoreClient customerClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await customerClient.GetVersionAsync(cancellationToken);

        result.ShouldNotBeNull();
    }
}
