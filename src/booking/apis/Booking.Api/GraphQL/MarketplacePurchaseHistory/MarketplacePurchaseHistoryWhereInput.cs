using Api.Shared.Services.Models;
using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[GraphQLName("MarketplacePurchaseHistoryWhereInput")]
public sealed class MarketplacePurchaseHistoryWhereInput
{
    public string? OrganizationCustomDomain { get; set; }
    public MarketplacePurchaseSourceType[]? SourceTypes { get; set; }
    public MarketplacePurchaseLifecycleState[]? LifecycleStates { get; set; }
    public PaymentStatus[]? PaymentStatuses { get; set; }
    public string? CustomerId { get; set; }
    public string? ProductVersionId { get; set; }
    public DateTimeOffset? ActivityFrom { get; set; }
    public DateTimeOffset? ActivityUntil { get; set; }
    public DateTimeOffset? BookingFrom { get; set; }
    public DateTimeOffset? BookingUntil { get; set; }
    public bool? IncludeMineOnly { get; set; }
}
