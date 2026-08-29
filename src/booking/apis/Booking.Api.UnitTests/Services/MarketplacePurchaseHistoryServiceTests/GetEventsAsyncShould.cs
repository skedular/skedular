using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;

namespace Booking.Api.UnitTests.Services.MarketplacePurchaseHistoryServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetEventsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Authorize_Subscription_Before_Reading_History(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IMarketplaceBookingSubscriptionService subscriptionService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => subscriptionService.GetByIdAsync("subscription-1", cancellationToken))
            .Returns(new MarketplaceBookingSubscription());
        A.CallTo(() => historyRepository.GetEventsAsync(
                MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
                "subscription-1",
                cancellationToken))
            .Returns([]);

        var result = await sut.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Subscription, "subscription-1", cancellationToken);

        result.ShouldBeEmpty();
        A.CallTo(() => subscriptionService.GetByIdAsync("subscription-1", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => historyRepository.GetEventsAsync(
            MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription,
            "subscription-1",
            cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Authorize_Entitlement_With_Current_Customer_Before_Reading_History(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IEntitlementPurchaseReadService purchaseReadService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => purchaseReadService.GetAuthorizedAsync("purchase-1", "customer-1", cancellationToken))
            .Returns(new EntitlementPurchase());
        A.CallTo(() => historyRepository.GetEventsAsync(
                MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase,
                "purchase-1",
                cancellationToken))
            .Returns([]);

        var result = await sut.GetEventsAsync(MarketplacePurchaseHistoryEligibleSourceType.Entitlement, "purchase-1", cancellationToken);

        result.ShouldBeEmpty();
        A.CallTo(() => purchaseReadService.GetAuthorizedAsync("purchase-1", "customer-1", cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
