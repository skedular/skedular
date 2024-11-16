using HotChocolate.Execution.Configuration;

namespace Location.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<LocationQuery>()
            .AddMutationType<LocationMutation>();
}
