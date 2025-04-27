using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Notification.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Notification.Api.GraphQL;

[GraphQLName("Notification")]
public class Notification : Node
{
    [GraphQLName("sourceId")] [ID] public string SourceId { get; set; } = string.Empty;
    [GraphQLName("eventRaisedAt")] public DateTimeOffset EventRaisedAt { get; set; }
    [GraphQLName("notificationType")] public NotificationType NotificationType { get; set; }
    [GraphQLName("invitedBy")] public CustomerDetails? InvitedBy { get; set; }
    [GraphQLName("invitee")] public CustomerDetails? Invitee { get; set; }
    [GraphQLName("organization")] public OrganizationDetails? Organization { get; set; }
    [GraphQLName("location")] public LocationDetails? Location { get; set; }
    [GraphQLName("team")] public TeamDetails? Team { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("NotificationConnection")]
public class NotificationConnection : Enterprise.Shared.GraphQL.Types.Connection<NotificationEdge>;

[GraphQLName("Notification_CustomerDetails")]
public class CustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }
    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }
    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }
    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }
    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }
    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }
    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
}

[GraphQLName("NotificationEdge")]
public class NotificationEdge(Notification node, string cursor) : Edge<Notification>(node, cursor);

[GraphQLName("Notification_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("NotificationOrderInput")]
public class NotificationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public NotificationOrderField Field { get; set; }
}

[GraphQLName("Notification_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("Notification_TeamDetails")]
public class TeamDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("MyNotificationWhereInput")]
public class MyNotificationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
}
