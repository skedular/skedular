using Customer.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Customer.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Customer.Api")]
public class CustomerMutationContractShould(ICustomerMutationContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Field_Selection(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var data = result.Data.ShouldNotBeNull();
        ShouldHaveFieldSelection(data.UpdateCustomerDetailsInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateMyCustomerDetailsInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateMyBillingDetailsInput?.InputFields?.Select(field => field.Name));
    }

    private static void ShouldHaveFieldSelection(IEnumerable<string>? fields)
    {
        fields.ShouldNotBeNull();
        fields.ShouldContain("fieldsToUpdate");
    }
}
