using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Models;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceRefundStatusExtensionsShould
{
    [Theory]
    [InlineData("Requested", MarketplaceRefundStatus.Requested, "Requested")]
    [InlineData("UnderReview", MarketplaceRefundStatus.UnderReview, "Under review")]
    [InlineData("ProviderPending", MarketplaceRefundStatus.ProviderPending, "Provider pending")]
    [InlineData("ReconciliationRequired", MarketplaceRefundStatus.ReconciliationRequired, "Reconciliation required")]
    [InlineData("Completed", MarketplaceRefundStatus.Completed, "Completed")]
    [InlineData("Approved", MarketplaceRefundStatus.Approved, "Approved")]
    [InlineData("Rejected", MarketplaceRefundStatus.Rejected, "Rejected")]
    [InlineData("Processing", MarketplaceRefundStatus.Processing, "Processing")]
    [InlineData("Failed", MarketplaceRefundStatus.Failed, "Failed")]
    [InlineData("Cancelled", MarketplaceRefundStatus.Cancelled, "Cancelled")]
    public void Map_Known_Statuses(string value, MarketplaceRefundStatus status, string displayName)
    {
        value.ToMarketplaceRefundStatus().ShouldBe(status);
        status.ToMarketplaceRefundStatusName().ShouldBe(displayName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("legacy-manual")]
    public void Default_Unknown_Status_To_Requested(string? value) => value.ToMarketplaceRefundStatus().ShouldBe(MarketplaceRefundStatus.Requested);
}
