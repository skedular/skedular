using HotChocolate.Execution.Configuration;

namespace Team.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<TeamQuery>()
            .AddMutationType<TeamMutation>();
}
