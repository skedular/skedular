using Api.Shared.Services.Models;

namespace Booking.Shared.Models;

public sealed class MarketplaceExternalRefundReconciliationModel
{
    public string Id { get; init; } = string.Empty;
    public string? OrganizationId { get; init; }
    public string? StripeAccountId { get; init; }
    public MarketplaceExternalRefundReconciliationProvider Provider { get; init; }
    public string ExternalRefundId { get; init; } = string.Empty;
    public decimal? Amount { get; init; }
    public Currency? Currency { get; init; }
    public MarketplaceExternalRefundReconciliationStatus Status { get; init; }
    public DateTimeOffset FirstSeenAt { get; init; }
    public DateTimeOffset LastSeenAt { get; init; }
    public int RetryCount { get; init; }
    public DateTimeOffset? NextRetryAt { get; init; }
    public string? ResolutionReason { get; init; }
    public string? ResolutionActorCustomerId { get; init; }
    public string? ResolutionCorrelationId { get; init; }
}
