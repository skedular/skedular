using HotChocolate.Execution.Configuration;

namespace Billing.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<BillingQuery>()
            .AddMutationType<BillingMutation>();
}
