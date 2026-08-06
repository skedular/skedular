using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryEdge")]
public sealed class MarketplacePurchaseHistoryEdge(MarketplacePurchaseHistoryDetails node, string cursor)
    : Edge<MarketplacePurchaseHistoryDetails>(node, cursor);
