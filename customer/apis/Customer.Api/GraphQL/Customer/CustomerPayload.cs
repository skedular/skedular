using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerPayload")]
public class CustomerPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
}
