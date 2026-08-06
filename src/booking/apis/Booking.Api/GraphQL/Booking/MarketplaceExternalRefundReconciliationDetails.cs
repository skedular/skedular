using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceExternalRefundReconciliationDetails")]
public class MarketplaceExternalRefundReconciliationDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("provider")]
    public string Provider { get; set; } = string.Empty;

    [GraphQLName("externalRefundId")]
    public string ExternalRefundId { get; set; } = string.Empty;

    [GraphQLName("amount")]
    public decimal? Amount { get; set; }

    [GraphQLName("currency")]
    public string? Currency { get; set; }

    [GraphQLName("status")]
    public string Status { get; set; } = string.Empty;

    [GraphQLName("firstSeenAt")]
    public DateTimeOffset FirstSeenAt { get; set; }

    [GraphQLName("lastSeenAt")]
    public DateTimeOffset LastSeenAt { get; set; }

    [GraphQLName("resolutionReason")]
    public string? ResolutionReason { get; set; }

    [GraphQLName("resolutionActorCustomerId")]
    public string? ResolutionActorCustomerId { get; set; }

    [GraphQLName("resolutionCorrelationId")]
    public string? ResolutionCorrelationId { get; set; }
}
