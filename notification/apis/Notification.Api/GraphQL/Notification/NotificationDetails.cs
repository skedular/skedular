using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("NotificationDetails")]
public class NotificationDetails : Node
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
