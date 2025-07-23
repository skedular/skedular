using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Sso;

[GraphQLName("OrganizationSsoSettingsDetails")]
public class OrganizationSsoSettingsDetails : Node
{
    [GraphQLName("isActive")] public bool IsActive { get; set; }
    [GraphQLName("entityId")] public string EntityId { get; set; } = string.Empty;
    [GraphQLName("loginUrl")] public string LoginUrl { get; set; } = string.Empty;

    [GraphQLName("appFederationMetadataUrl")]
    public string AppFederationMetadataUrl { get; set; } = string.Empty;

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
