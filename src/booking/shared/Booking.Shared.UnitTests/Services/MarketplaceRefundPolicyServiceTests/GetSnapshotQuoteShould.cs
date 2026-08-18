using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundPolicyServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetSnapshotQuoteShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_Snapshot_Rules_Independently_Of_Live_Pricing(MarketplaceRefundPolicyService sut)
    {
        var snapshot = new CancellationPolicySnapshot(
            "TieredRefund",
            [new CancellationRefundRuleSnapshot(60, 75)],
            TimeProvider.System.GetUtcNow().AddDays(-10),
            "price-at-purchase");

        var result = sut.GetQuote(snapshot, TimeProvider.System.GetUtcNow().AddHours(2), TimeProvider.System.GetUtcNow());

        result.IsRefundable.ShouldBeTrue();
        result.RefundPercentage.ShouldBe(75);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Snapshot_Rule_At_The_Exact_Cutoff(MarketplaceRefundPolicyService sut)
    {
        var referenceTime = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero);
        var snapshot = new CancellationPolicySnapshot(
            "TieredRefund",
            [new CancellationRefundRuleSnapshot(60, 80)],
            referenceTime,
            "price-at-purchase");

        var result = sut.GetQuote(snapshot, referenceTime, referenceTime.AddMinutes(-60));

        result.IsRefundable.ShouldBeTrue();
        result.RefundPercentage.ShouldBe(80);
        result.AppliedRuleMinutesBefore.ShouldBe(60);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Clamp_Snapshot_Refund_Percentage(MarketplaceRefundPolicyService sut)
    {
        var referenceTime = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero);
        var snapshot = new CancellationPolicySnapshot(
            "TieredRefund",
            [new CancellationRefundRuleSnapshot(0, 150)],
            referenceTime,
            "price-at-purchase");

        sut.GetQuote(snapshot, referenceTime, referenceTime).RefundPercentage.ShouldBe(100);
    }
}
