using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundKindDetails")]
public class MarketplaceRefundKindDetails
{
    public MarketplaceRefundKind Type { get; set; }
    public string Name { get; set; } = string.Empty;
}
