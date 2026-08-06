using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Models.MarketplacePurchaseHistoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchaseHistoryShould
{
    [Fact]
    public void Expose_Stable_Display_Names_For_OneTime_Bookings()
    {
        var entry = new MarketplacePurchaseHistoryEntry(
            "booking-1",
            MarketplacePurchaseSourceType.Booking,
            MarketplacePurchaseLifecycleState.Cancelled,
            MarketplacePurchaseRenewalState.NotApplicable,
            TimeProvider.System.GetUtcNow(),
            TimeProvider.System.GetUtcNow(),
            null,
            null,
            PaymentStatus.Confirmed,
            null,
            null,
            null,
            null,
            null,
            false);

        entry.SourceTypeName.ShouldBe("One-time booking");
        entry.LifecycleStateName.ShouldBe("Canceled");
        entry.RenewalStateName.ShouldBe("Not applicable");
    }

    [Theory]
    [InlineData(MarketplacePurchaseRenewalState.Renews, "Renews")]
    [InlineData(MarketplacePurchaseRenewalState.DoesNotRenew, "Does not renew")]
    [InlineData(MarketplacePurchaseRenewalState.NotApplicable, "Not applicable")]
    public void Map_Renewal_State_To_Operator_Label(MarketplacePurchaseRenewalState state, string expected)
    {
        var entry = new MarketplacePurchaseHistoryEntry(
            "purchase-1",
            MarketplacePurchaseSourceType.Subscription,
            MarketplacePurchaseLifecycleState.Active,
            state,
            TimeProvider.System.GetUtcNow(),
            TimeProvider.System.GetUtcNow(),
            null,
            null,
            PaymentStatus.Confirmed,
            null,
            null,
            null,
            null,
            null,
            false);

        entry.RenewalStateName.ShouldBe(expected);
    }

    [Theory]
    [InlineData(MarketplacePurchaseLifecycleState.Active, "Active")]
    [InlineData(MarketplacePurchaseLifecycleState.Cancelled, "Canceled")]
    [InlineData(MarketplacePurchaseLifecycleState.Deleted, "Deleted")]
    [InlineData(MarketplacePurchaseLifecycleState.Expired, "Expired")]
    [InlineData(MarketplacePurchaseLifecycleState.PaymentFailed, "Payment failed")]
    [InlineData(MarketplacePurchaseLifecycleState.Pending, "Pending")]
    public void Map_Lifecycle_State_To_Operator_Label(MarketplacePurchaseLifecycleState state, string expected)
    {
        var entry = new MarketplacePurchaseHistoryEntry(
            "purchase-1",
            MarketplacePurchaseSourceType.Subscription,
            state,
            MarketplacePurchaseRenewalState.NotApplicable,
            TimeProvider.System.GetUtcNow(),
            TimeProvider.System.GetUtcNow(),
            null,
            null,
            PaymentStatus.Pending,
            null,
            null,
            null,
            null,
            null,
            state == MarketplacePurchaseLifecycleState.Deleted);

        entry.LifecycleStateName.ShouldBe(expected);
    }
}
