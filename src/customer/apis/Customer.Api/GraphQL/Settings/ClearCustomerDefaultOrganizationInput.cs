using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("ClearCustomerDefaultOrganizationInput")]
public class ClearCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
