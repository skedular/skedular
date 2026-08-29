using Api.Shared.Services.Models;
using Booking.Api.GraphQL.EntitlementPurchase;
using Booking.Api.Services;
using Booking.Shared.Models;

namespace Booking.Api.UnitTests.GraphQL.MarketplacePurchaseHistory;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EntitlementHistoryShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Entitlement_Event_Payload(
        [Frozen]
        IMarketplacePurchaseHistoryService historyService,
        CancellationToken cancellationToken)
    {
        var eventModel = new MarketplacePurchaseHistoryEventModel(
            "event-1", "purchase-1", MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
            MarketplacePurchaseHistoryEventType.CreditsConsumed, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow,
            null, PaymentStatus.Confirmed, null, null, null, 2, 8, null, null, null, null, null, "Booking used credits");
        A.CallTo(() => historyService.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Entitlement, "purchase-1", cancellationToken))
            .Returns([eventModel]);

        var sut = new EntitlementPurchaseDetails
        {
            Id = "purchase-1",
        };
        var result = await sut.GetHistoryAsync(null, 10, null, null, historyService, cancellationToken);

        result.TotalCount.ShouldBe(1);
        result.Edges.Single().Node.Type.ShouldBe(MarketplacePurchaseHistoryEventType.CreditsConsumed);
        result.Edges.Single().Node.CreditQuantity.ShouldBe(2);
        result.Edges.Single().Node.RemainingCreditQuantity.ShouldBe(8);
        result.Edges.Single().Node.Name.ShouldBe("Credits consumed");
    }
}
