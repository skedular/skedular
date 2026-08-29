using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplacePurchaseHistoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EntitlementLifecycleHistoryShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Append_Each_Entitlement_Lifecycle_Event_With_Stable_Key(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        MarketplacePurchaseHistoryEventService sut,
        MarketplacePurchaseHistoryEventType eventType,
        CancellationToken cancellationToken)
    {
        eventType = eventType switch
        {
            MarketplacePurchaseHistoryEventType.SubscriptionStarted or
                MarketplacePurchaseHistoryEventType.SubscriptionRenewed or
                MarketplacePurchaseHistoryEventType.CancellationScheduled or
                MarketplacePurchaseHistoryEventType.CancellationCompleted => MarketplacePurchaseHistoryEventType.PurchaseCreated,
            _ => eventType,
        };
        var eventModel = CreateEvent(eventType);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => historyRepository.AppendEventAsync(eventModel, "entitlement-event-key", cancellationToken)).Returns(eventModel);

        await sut.AppendAsync(eventModel, "entitlement-event-key", cancellationToken);

        A.CallTo(() => historyRepository.AppendEventAsync(eventModel, "entitlement-event-key", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static MarketplacePurchaseHistoryEventModel CreateEvent(MarketplacePurchaseHistoryEventType type) => new(
        "event-1", "purchase-1", MarketplacePurchaseHistoryEligibleSourceType.Entitlement, type,
        DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, null, null, null, null, null, null, null, null, null, null, null, null, null);
}
