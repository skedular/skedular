using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("EntitlementDetails")]
public sealed class EntitlementDetails
{
    public string Id { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationCustomDomain { get; set; } = string.Empty;
    public string PurchaseReference { get; set; } = string.Empty;
    public string PricingId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int GrantedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int? RemainingWeeklyRedemptions { get; set; }
    public DateTimeOffset ActivatesAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public EntitlementStatus Status { get; set; }
    public string Currency { get; set; } = string.Empty;
    public EntitlementRestrictionsDetails? Restrictions { get; set; }
    public string? PaymentAction { get; set; }
    public EntitlementRefundDetails? Refund { get; set; }
    public IReadOnlyList<CreditLedgerEntryDetails> Ledger { get; set; } = [];
    public IReadOnlyList<string> LinkedBookingIds { get; set; } = [];
}

[GraphQLName("EntitlementRestrictionsDetails")]
public sealed class EntitlementRestrictionsDetails
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductVersionId { get; set; } = string.Empty;
    public IReadOnlyList<DayOfWeek> AvailableDays { get; set; } = [];
    public int? MinDurationMinutes { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public int NumberOfResourcesToBook { get; set; }
}

[GraphQLName("EntitlementRefundDetails")]
public sealed class EntitlementRefundDetails
{
    public string Id { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int UnusedCreditQuantity { get; set; }
    public MarketplaceRefundStatus Status { get; set; }
    public string? PaymentRefundStatus { get; set; }
}

[GraphQLName("CreditLedgerEntryDetails")]
public sealed class CreditLedgerEntryDetails
{
    public string Id { get; set; } = string.Empty;
    public string? BookingId { get; set; }
    public int Quantity { get; set; }
    public CreditLedgerTransactionType TransactionType { get; set; }
    public string ReferenceKey { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
