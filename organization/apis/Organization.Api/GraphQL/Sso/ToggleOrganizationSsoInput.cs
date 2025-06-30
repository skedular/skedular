using HotChocolate;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("ToggleOrganizationSsoInput")]
public class ToggleOrganizationSsoInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("isActive")] public bool IsActive { get; set; }
}
