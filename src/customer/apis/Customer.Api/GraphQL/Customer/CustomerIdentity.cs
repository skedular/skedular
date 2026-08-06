using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerIdentity")]
public class CustomerIdentity : Node
{
    [GraphQLName("email")]
    public string? Email { get; set; }

    [GraphQLName("verified")]
    public bool Verified { get; set; }
}
