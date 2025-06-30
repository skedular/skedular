using HotChocolate;
using HotChocolate.Types.Relay;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("Marketplace_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("tagType")] public string? TagType { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
