using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("AddLocationTagInput")]
public class AddLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("color")] public string? Color { get; set; }
}
