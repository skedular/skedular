using Marketplace.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Marketplace.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Marketplace.Api")]
public class MarketplaceMutationContractShould(IMarketplaceMutationContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Field_Selection(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var fields = result.Data.ShouldNotBeNull().UpdateProductInput?.InputFields?.Select(field => field.Name);
        fields.ShouldNotBeNull();
        fields.ShouldContain("fieldsToUpdate");
    }
}
