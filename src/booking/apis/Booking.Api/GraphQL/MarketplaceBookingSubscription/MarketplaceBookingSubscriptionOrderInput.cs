using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionOrderInput")]
public class MarketplaceBookingSubscriptionOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public MarketplaceBookingSubscriptionOrderField Field { get; set; }
}
