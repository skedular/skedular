using HotChocolate;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("UpdateOrganizationSsoSettingsPayload")]
public class UpdateOrganizationSsoSettingsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
}
