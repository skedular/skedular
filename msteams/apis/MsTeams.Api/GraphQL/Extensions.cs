using HotChocolate.Execution.Configuration;

namespace MsTeams.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<MsTeamsQuery>();
}
