using Api.Shared.Clients.OpenApi.Skedular.Customer.V1;
using Customer.Domain.IntegrationTests.Clients.GraphQL.V1;
using Shouldly;
using Testing.Shared;

namespace Customer.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class GetVersionShould(IGetVersionQuery getVersionQuery)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Version(CancellationToken cancellationToken)
    {
        var result = await getVersionQuery.ExecuteAsync(cancellationToken);

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.CustomerVersion.ShouldNotBeNull();
    }
}
