using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id, string customDomain) : Node(id)
{
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; } = customDomain;
}
