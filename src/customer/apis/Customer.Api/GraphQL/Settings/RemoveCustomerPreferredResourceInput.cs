using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("RemoveCustomerPreferredResourceInput")]
public class RemoveCustomerPreferredResourceInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("resourceId")]
    public string ResourceId { get; set; } = string.Empty;
}
