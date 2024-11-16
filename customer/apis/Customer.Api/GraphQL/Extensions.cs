using HotChocolate.Execution.Configuration;

namespace Customer.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<CustomerQuery>()
            .AddMutationType<CustomerMutation>();
}
