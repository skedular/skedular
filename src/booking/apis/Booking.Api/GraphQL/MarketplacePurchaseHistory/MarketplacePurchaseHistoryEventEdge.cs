using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryEventEdge")]
public sealed class MarketplacePurchaseHistoryEventEdge(
    MarketplacePurchaseHistoryEventDetails node,
    string cursor) : Edge<MarketplacePurchaseHistoryEventDetails>(node, cursor);
