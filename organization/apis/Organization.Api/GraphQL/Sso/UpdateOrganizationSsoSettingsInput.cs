using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("UpdateOrganizationSsoSettingsInput")]
public class UpdateOrganizationSsoSettingsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("entityId")] public string EntityId { get; set; } = string.Empty;
    [GraphQLName("loginUrl")] public string LoginUrl { get; set; } = string.Empty;

    [GraphQLName("appFederationMetadataUrl")]
    public string AppFederationMetadataUrl { get; set; } = string.Empty;

    [GraphQLName("isActive")] public bool IsActive { get; set; } = true;
}
