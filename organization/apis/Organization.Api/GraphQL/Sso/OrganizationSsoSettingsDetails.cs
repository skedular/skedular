using HotChocolate;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("OrganizationSsoSettingsDetails")]
public class OrganizationSsoSettingsDetails
{
    [GraphQLName("entityId")] public string EntityId { get; set; } = string.Empty;
    [GraphQLName("loginUrl")] public string LoginUrl { get; set; } = string.Empty;

    [GraphQLName("appFederationMetadataUrl")]
    public string AppFederationMetadataUrl { get; set; } = string.Empty;
}
