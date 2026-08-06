namespace Booking.Shared.Models;

public enum MarketplaceRefundKind { Cancellation, Modification, Partial }

public static class MarketplaceRefundKindConstants
{
    public const string Cancellation = "Cancellation";
    public const string Modification = "Modification";
    public const string Partial = "Partial";
}

public static class MarketplaceRefundKindExtensions
{
    public static MarketplaceRefundKind ToMarketplaceRefundKind(this string? value) =>
        value switch
        {
            MarketplaceRefundKindConstants.Modification => MarketplaceRefundKind.Modification,
            MarketplaceRefundKindConstants.Partial => MarketplaceRefundKind.Partial,
            _ => MarketplaceRefundKind.Cancellation,
        };
}
