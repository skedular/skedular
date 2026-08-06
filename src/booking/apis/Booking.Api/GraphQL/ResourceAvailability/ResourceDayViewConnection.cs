using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("ResourceDayViewConnection")]
public class ResourceDayViewConnection
{
    [GraphQLName("items")]
    public IEnumerable<ResourceDayViewDetails> Items { get; set; } = [];

    [GraphQLName("subscriptionKey")]
    public string SubscriptionKey { get; set; } = string.Empty;
}
