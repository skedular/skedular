using Booking.Shared.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

public sealed class MarketplacePurchaseHistoryOrderInput
{
    public MarketplacePurchaseHistoryOrderField Field { get; set; } = MarketplacePurchaseHistoryOrderField.ActivityAt;
    public OrderDirection Direction { get; set; } = OrderDirection.Descending;
}
