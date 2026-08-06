using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundEventTypeDetails")]
public class MarketplaceRefundEventTypeDetails
{
    [GraphQLName("type")]
    public MarketplaceRefundEventType Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("MarketplaceRefundEventDetails")]
public class MarketplaceRefundEventDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("eventType")]
    public MarketplaceRefundEventTypeDetails EventType { get; set; } = new();

    [GraphQLName("occurredAt")]
    public DateTimeOffset OccurredAt { get; set; }

    [GraphQLName("refundAmount")]
    public decimal? RefundAmount { get; set; }

    [GraphQLName("currencyToDisplay")]
    public string CurrencyToDisplay { get; set; } = string.Empty;

    [GraphQLName("reason")]
    public string? Reason { get; set; }

    [GraphQLName("accountingProvider")]
    public string? AccountingProvider { get; set; }

    [GraphQLName("externalRefundId")]
    public string? ExternalRefundId { get; set; }

    [GraphQLName("externalRefundNumber")]
    public string? ExternalRefundNumber { get; set; }

    [GraphQLName("lastError")]
    public string? LastError { get; set; }

    [GraphQLName("actorCustomerId")]
    public string? ActorCustomerId { get; set; }

    [GraphQLName("actorName")]
    public string? ActorName { get; set; }

    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? CorrelationId { get; set; }
}
