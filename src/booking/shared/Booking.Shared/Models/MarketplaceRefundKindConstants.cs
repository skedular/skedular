namespace Booking.Shared.Models;

public enum MarketplaceRefundKind { Cancellation, Modification, Partial, EntitlementExpiry }

public static class MarketplaceRefundKindConstants
{
    public const string Cancellation = "Cancellation";
    public const string Modification = "Modification";
    public const string Partial = "Partial";
    public const string EntitlementExpiry = "EntitlementExpiry";
}

public static class MarketplaceRefundKindExtensions
{
    public static MarketplaceRefundKind ToMarketplaceRefundKind(this string? value) =>
        value switch
        {
            MarketplaceRefundKindConstants.Modification => MarketplaceRefundKind.Modification,
            MarketplaceRefundKindConstants.Partial => MarketplaceRefundKind.Partial,
            MarketplaceRefundKindConstants.EntitlementExpiry => MarketplaceRefundKind.EntitlementExpiry,
            _ => MarketplaceRefundKind.Cancellation,
        };
}
