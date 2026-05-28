using Api.Shared.Grpc.Skedular.Customer.Core.V1;

namespace Customer.Domain.IntegrationTests.Api.Grpc.CustomerGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class GetVersionShould(CustomerService.CustomerServiceClient customerServiceClient)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await customerServiceClient.GetVersionAsync(new VersionInput(), cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
    }
}
