using HotChocolate;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("RemoveOrganizationSsoSettingsPayload")]
public class RemoveOrganizationSsoSettingsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
}
