using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationPayload")]
public class OrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
}
