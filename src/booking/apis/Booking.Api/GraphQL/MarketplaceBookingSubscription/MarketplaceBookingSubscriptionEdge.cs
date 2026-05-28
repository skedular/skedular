using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionEdge")]
public class MarketplaceBookingSubscriptionEdge(MarketplaceBookingSubscriptionDetails node, string cursor)
    : Edge<MarketplaceBookingSubscriptionDetails>(node, cursor);
