using HotChocolate.Execution.Configuration;

namespace Slack.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<SlackQuery>();
}
