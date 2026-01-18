using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id, string uniqueAlphanumericName) : Node(id)
{
    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; } = uniqueAlphanumericName;
}
