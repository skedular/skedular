using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models.Entitlements;

public sealed class EntitlementModel : ModelBase
{
    public EntitlementStatus LifecycleState { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationCustomDomain { get; set; } = string.Empty;
    public string PurchaseReference { get; set; } = string.Empty;
    public string PricingId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductVersionId { get; set; } = string.Empty;
    public int GrantedQuantity { get; set; }
    public DateTimeOffset ActivatesAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public EntitlementStatus Status { get; set; }
    public decimal NetPurchaseAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public ProductPricing? ProductPricing { get; set; }
    public EntitlementRefundModel? Refund { get; set; }
    public IReadOnlyList<CreditLedgerEntryModel> LedgerEntries { get; set; } = [];
    public IReadOnlyList<string> LinkedBookingIds { get; set; } = [];
}

public sealed class EntitlementRefundModel
{
    public string Id { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int UnusedCreditQuantity { get; set; }
    public MarketplaceRefundStatus Status { get; set; }
    public string? PaymentRefundStatus { get; set; }
}

public sealed class CreditLedgerEntryModel : ModelBase
{
    public string EntitlementId { get; set; } = string.Empty;
    public string? BookingId { get; set; }
    public int Quantity { get; set; }
    public CreditLedgerTransactionType TransactionType { get; set; }
    public string ReferenceKey { get; set; } = string.Empty;
    public string? ActorOrSource { get; set; }
    public string? MetadataJson { get; set; }
}
