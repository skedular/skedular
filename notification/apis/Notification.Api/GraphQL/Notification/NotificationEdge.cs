using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("NotificationEdge")]
public class NotificationEdge(NotificationDetails node, string cursor) : Edge<NotificationDetails>(node, cursor);
