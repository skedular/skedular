using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionStatusDetails")]
public class MarketplaceBookingSubscriptionStatusDetails
{
    [GraphQLName("type")]
    public MarketplaceBookingSubscriptionStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
