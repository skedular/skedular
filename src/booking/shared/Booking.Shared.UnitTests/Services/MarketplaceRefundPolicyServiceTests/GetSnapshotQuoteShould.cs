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
}
