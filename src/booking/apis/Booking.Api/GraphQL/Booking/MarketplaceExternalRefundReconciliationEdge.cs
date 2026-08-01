using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceExternalRefundReconciliationEdge")]
public class MarketplaceExternalRefundReconciliationEdge(
    MarketplaceExternalRefundReconciliationDetails node,
    string cursor) : Edge<MarketplaceExternalRefundReconciliationDetails>(node, cursor);
