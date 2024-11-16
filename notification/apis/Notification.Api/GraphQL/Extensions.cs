using HotChocolate.Execution.Configuration;

namespace Notification.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<NotificationQuery>();
}
