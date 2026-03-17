using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionPayload")]
public class MarketplaceBookingSubscriptionPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("marketplaceBookingSubscription")]
    public MarketplaceBookingSubscriptionDetails MarketplaceBookingSubscription { get; set; } = new();
}
