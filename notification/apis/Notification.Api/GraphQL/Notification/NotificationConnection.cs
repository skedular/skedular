using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("NotificationConnection")]
public class NotificationConnection : Connection<NotificationEdge>;
