using HotChocolate;
using HotChocolate.Types.Relay;

namespace Notification.Api.GraphQL;

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("Notification")]
public class Notification : Node
{
    [GraphQLName("sourceId")] [ID] public string SourceId { get; set; }

    [GraphQLName("eventRaisedAt")] public DateTimeOffset EventRaisedAt { get; set; }

    [GraphQLName("notificationType")] public NotificationNotificationType NotificationType { get; set; }

    [GraphQLName("invitedBy")] public NotificationCustomerDetails? InvitedBy { get; set; }

    [GraphQLName("invitee")] public NotificationCustomerDetails? Invitee { get; set; }

    [GraphQLName("organization")] public NotificationOrganizationDetails? Organization { get; set; }

    [GraphQLName("location")] public NotificationLocationDetails? Location { get; set; }

    [GraphQLName("team")] public NotificationTeamDetails? Team { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("NotificationConnection")]
public class NotificationConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public NotificationEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("NotificationCustomerDetails")]
public class NotificationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

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

    [GraphQLName("emails")] public string[] Emails { get; set; }
}

[GraphQLName("NotificationEdge")]
public class NotificationEdge
{
    [GraphQLName("node")] public Notification Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

[GraphQLName("NotificationLocationDetails")]
public class NotificationLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }
}

public enum NotificationNotificationType
{
    InvitationToJoinOrganization,
    InvitationToJoinLocation,
    InvitationToJoinTeam
}

public enum NotificationOrderField
{
    EventRaisedAt,
    NotificationType
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
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("NotificationTeamDetails")]
public class NotificationTeamDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }
}

public enum OrderDirection
{
    Ascending,
    Descending
}

[GraphQLName("PageInfo")]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }

    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }

    [GraphQLName("startCursor")] public string? StartCursor { get; set; }

    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
