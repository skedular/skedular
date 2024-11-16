using HotChocolate.Execution.Configuration;

namespace Payment.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<PaymentQuery>()
            .AddMutationType<PaymentMutation>();
}
