using HotChocolate.Execution.Configuration;

namespace Booking.Api.GraphQL;

public static class Extensions
{
    public static IRequestExecutorBuilder AddTypes(this IRequestExecutorBuilder builder) =>
        builder
            .AddQueryType<BookingQuery>()
            .AddMutationType<BookingMutation>();
}
