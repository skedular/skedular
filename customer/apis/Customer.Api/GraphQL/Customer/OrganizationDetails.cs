using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id, string customDomain) : Node(id)
{
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; } = customDomain;
}
