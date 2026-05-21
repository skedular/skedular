using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("UpdateOrganizationTagInput")]
public class UpdateOrganizationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("fieldsToUpdate")] public HashSet<OrganizationTagPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
