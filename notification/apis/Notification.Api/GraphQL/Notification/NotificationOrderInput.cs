using Enterprise.Shared.Pagination;
using HotChocolate;
using Notification.Shared.Models;

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("NotificationOrderInput")]
public class NotificationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public NotificationOrderField Field { get; set; }
}
