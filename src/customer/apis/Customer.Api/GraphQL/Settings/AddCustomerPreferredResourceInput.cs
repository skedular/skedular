using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("AddCustomerPreferredResourceInput")]
public class AddCustomerPreferredResourceInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("resourceId")]
    public string ResourceId { get; set; } = string.Empty;
}
