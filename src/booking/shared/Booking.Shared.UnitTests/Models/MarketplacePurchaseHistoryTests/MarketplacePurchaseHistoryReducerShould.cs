using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Models.MarketplacePurchaseHistoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchaseHistoryReducerShould
{
    [Theory]
    [InlineData(MarketplacePurchaseHistoryEventTypeConstants.PurchaseCreated, MarketplacePurchaseHistoryEventType.PurchaseCreated)]
    [InlineData(MarketplacePurchaseHistoryEventTypeConstants.SubscriptionRenewed, MarketplacePurchaseHistoryEventType.SubscriptionRenewed)]
    [InlineData(MarketplacePurchaseHistoryEventTypeConstants.CreditsConsumed, MarketplacePurchaseHistoryEventType.CreditsConsumed)]
    public void Map_Persisted_Event_Type_Explicitly(string persisted, MarketplacePurchaseHistoryEventType expected) =>
        persisted.ToEventType().ShouldBe(expected);

    [Fact]
    public void Reject_Unknown_Persisted_Event_Type() => Should.Throw<ArgumentOutOfRangeException>(() => "UNKNOWN".ToEventType());

    [Fact]
    public void Reduce_Events_In_Occurrence_Order()
    {
        var later = new MarketplacePurchaseHistoryEventModel(
            "2", "purchase-1", MarketplacePurchaseHistoryEligibleSourceType.Subscription,
            MarketplacePurchaseHistoryEventType.PaymentStateChanged,
            DateTimeOffset.Parse("2026-01-02T00:00:00Z"), DateTimeOffset.Parse("2026-01-02T00:00:01Z"),
            PaymentStatus.Pending, PaymentStatus.Confirmed, null, null, null, null, null, null, null, null, null, null, null);
        var earlier = later with
        {
            Id = "1",
            Type = MarketplacePurchaseHistoryEventType.PurchaseCreated,
            OccurredAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            RecordedAt = DateTimeOffset.Parse("2026-01-01T00:00:01Z"),
            PaymentStatus = PaymentStatus.Pending,
        };

        var result = MarketplacePurchaseHistoryReducer.Reduce([later, earlier]);

        result.PurchasedAt.ShouldBe(earlier.OccurredAt);
        result.PaymentStatus.ShouldBe(PaymentStatus.Confirmed);
        result.ActivityAt.ShouldBe(later.OccurredAt);
    }
}
