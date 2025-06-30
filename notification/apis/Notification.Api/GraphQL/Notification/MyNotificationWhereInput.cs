using HotChocolate;

namespace Notification.Api.GraphQL.Notification;

[GraphQLName("MyNotificationWhereInput")]
public class MyNotificationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
}
