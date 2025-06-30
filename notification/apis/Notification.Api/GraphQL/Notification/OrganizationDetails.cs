using HotChocolate;
using HotChocolate.Types.Relay;

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("Notification_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}
