using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("tagType")] public string TagType { get; set; } = string.Empty;
    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
