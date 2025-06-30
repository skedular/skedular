using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("DeleteOrganizationInput")]
public class DeleteOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
