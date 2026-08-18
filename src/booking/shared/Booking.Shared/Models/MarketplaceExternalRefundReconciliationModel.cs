using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public sealed class MarketplaceExternalRefundReconciliationModel : ModelBase
{
    public string? OrganizationId { get; set; }
    public string? StripeAccountId { get; set; }
    public MarketplaceExternalRefundReconciliationProvider Provider { get; set; }
    public string ExternalRefundId { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public Currency? Currency { get; set; }
    public MarketplaceExternalRefundReconciliationStatus Status { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? NextRetryAt { get; set; }
    public string? ResolutionReason { get; set; }
    public string? ResolutionActorCustomerId { get; set; }
    public string? ResolutionCorrelationId { get; set; }
}
