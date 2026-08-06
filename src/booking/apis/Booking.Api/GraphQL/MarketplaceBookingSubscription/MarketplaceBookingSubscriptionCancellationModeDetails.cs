using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionCancellationModeDetails")]
public class MarketplaceBookingSubscriptionCancellationModeDetails
{
    [GraphQLName("type")]
    public MarketplaceBookingSubscriptionCancellationMode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
