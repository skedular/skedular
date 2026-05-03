using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("OrganizationTagDetails")]
[Shareable]
public class OrganizationTagDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("type")] public OrganizationTagType? Type { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
