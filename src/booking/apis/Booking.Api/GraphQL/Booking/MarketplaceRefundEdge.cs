using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundEdge")]
public sealed class MarketplaceRefundEdge(MarketplaceRefundDetails node, string cursor)
    : Edge<MarketplaceRefundDetails>(node, cursor);
