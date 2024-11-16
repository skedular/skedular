using HotChocolate.Execution.Configuration;

namespace Organization.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<OrganizationQuery>()
            .AddMutationType<OrganizationMutation>();
}
