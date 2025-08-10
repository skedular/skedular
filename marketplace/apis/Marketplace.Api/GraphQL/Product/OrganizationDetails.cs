using HotChocolate;
using HotChocolate.Types.Relay;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("Marketplace_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;

    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; }
}
