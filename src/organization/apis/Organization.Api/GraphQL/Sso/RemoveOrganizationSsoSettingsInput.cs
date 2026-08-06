using HotChocolate;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("RemoveOrganizationSsoSettingsInput")]
public class RemoveOrganizationSsoSettingsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }
}
