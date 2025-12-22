using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerIdentity")]
public class CustomerIdentity : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("verified")] public bool Verified { get; set; }
    [GraphQLName("type")] public IdentityType Type { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
