using HotChocolate;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("RemoveOrganizationSsoSettingsInput")]
public class RemoveOrganizationSsoSettingsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }
}
