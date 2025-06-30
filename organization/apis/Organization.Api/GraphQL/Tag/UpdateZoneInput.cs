using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("UpdateZoneInput")]
public class UpdateZoneInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
