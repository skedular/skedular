using Booking.Api.GraphQL.EntitlementPurchase;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Services;
using Booking.Shared.Models;

namespace Booking.Api.UnitTests.GraphQL.MarketplacePurchaseHistory;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchaseHistoryEventsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Subscription_Events_In_Backend_Order(
        [Frozen]
        IMarketplacePurchaseHistoryService historyService,
        CancellationToken cancellationToken)
    {
        var events = new[]
        {
            CreateEvent("newest", MarketplacePurchaseHistoryEventType.SubscriptionRenewed),
            CreateEvent("older", MarketplacePurchaseHistoryEventType.PurchaseCreated),
        };
        A.CallTo(() => historyService.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Subscription, "subscription-1", cancellationToken))
            .Returns(events);

        var sut = new MarketplaceBookingSubscriptionDetails
        {
            Id = "subscription-1",
        };
        var result = await sut.GetHistoryAsync(null, 10, null, null, historyService, cancellationToken);

        result.TotalCount.ShouldBe(2);
        result.Edges.Select(item => item.Node.Id).ShouldBe(["newest", "older"]);
        result.Edges.Select(item => item.Node.Type).ShouldBe([
            MarketplacePurchaseHistoryEventType.SubscriptionRenewed,
            MarketplacePurchaseHistoryEventType.PurchaseCreated,
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_Entitlement_History(
        [Frozen]
        IMarketplacePurchaseHistoryService historyService,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => historyService.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Entitlement, "purchase-1", cancellationToken))
            .Returns([]);

        var sut = new EntitlementPurchaseDetails
        {
            Id = "purchase-1",
        };
        var result = await sut.GetHistoryAsync(null, 10, null, null, historyService, cancellationToken);

        result.TotalCount.ShouldBe(0);
        result.Edges.ShouldBeEmpty();
        result.PageInfo.HasNextPage.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Events_Immediately_Before_Cursor_For_Last_Page(
        [Frozen]
        IMarketplacePurchaseHistoryService historyService,
        CancellationToken cancellationToken)
    {
        var events = Enumerable.Range(0, 5)
            .Select(index => CreateEvent($"event-{index}", MarketplacePurchaseHistoryEventType.PurchaseCreated))
            .ToArray();
        A.CallTo(() => historyService.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Subscription, "subscription-1", cancellationToken))
            .Returns(events);

        var sut = new MarketplaceBookingSubscriptionDetails
        {
            Id = "subscription-1",
        };
        var result = await sut.GetHistoryAsync(null, null, "3", 2, historyService, cancellationToken);

        result.Edges.Select(item => item.Node.Id).ShouldBe(["event-1", "event-2"]);
        result.PageInfo.HasPreviousPage.ShouldBeTrue();
        result.PageInfo.HasNextPage.ShouldBeTrue();
    }

    private static MarketplacePurchaseHistoryEventModel CreateEvent(string id, MarketplacePurchaseHistoryEventType type) => new(
        id,
        "source-1",
        type is MarketplacePurchaseHistoryEventType.EntitlementCreated or MarketplacePurchaseHistoryEventType.EntitlementExpired
            or MarketplacePurchaseHistoryEventType.CreditsConsumed
            ? MarketplacePurchaseHistoryEligibleSourceType.Entitlement
            : MarketplacePurchaseHistoryEligibleSourceType.Subscription,
        type,
        DateTimeOffset.UtcNow,
        DateTimeOffset.UtcNow,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null);
}
