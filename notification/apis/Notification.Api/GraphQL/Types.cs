using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Relay;
using Notification.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Notification.Api.GraphQL;

[GraphQLName("Notification")]
public class Notification : Node
{
    [GraphQLName("sourceId")] [ID] public string SourceId { get; set; } = string.Empty;
    [GraphQLName("eventRaisedAt")] public DateTimeOffset EventRaisedAt { get; set; }
    [GraphQLName("notificationType")] public NotificationType NotificationType { get; set; }
    [GraphQLName("invitedBy")] public NotificationCustomerDetails? InvitedBy { get; set; }
    [GraphQLName("invitee")] public NotificationCustomerDetails? Invitee { get; set; }
    [GraphQLName("organization")] public NotificationOrganizationDetails? Organization { get; set; }
    [GraphQLName("location")] public NotificationLocationDetails? Location { get; set; }
    [GraphQLName("team")] public NotificationTeamDetails? Team { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("NotificationConnection")]
public class NotificationConnection : Connection<NotificationEdge>;

[GraphQLName("NotificationCustomerDetails")]
public class NotificationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
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
    [GraphQLName("emails")] public string[] Emails { get; set; } = [];
}

[GraphQLName("NotificationEdge")]
public class NotificationEdge : Edge<Notification>;

[GraphQLName("NotificationLocationDetails")]
public class NotificationLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("NotificationOrderInput")]
public class NotificationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public NotificationOrderField Field { get; set; }
}

[GraphQLName("NotificationOrganizationDetails")]
public class NotificationOrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("NotificationTeamDetails")]
public class NotificationTeamDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
