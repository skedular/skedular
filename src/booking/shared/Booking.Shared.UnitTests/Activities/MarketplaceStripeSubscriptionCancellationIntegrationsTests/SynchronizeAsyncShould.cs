using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.MarketplaceStripeSubscriptionCancellationIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SynchronizeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnoreReservationWithoutStripeSubscription(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository subscriptionRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceStripeSubscriptionCancellationIntegrations sut)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1",
            StripeAccountId = "acct-1",
            StripeSubscriptionId = null,
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(subscriptionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => subscriptionRepository.GetByIdAsync(subscription.Id, A<CancellationToken>._)).Returns(subscription);
        var input = new MarketplaceStripeSubscriptionCancellationInput("subscription", subscription.Id, false);
        var activityEnvironment = new ActivityEnvironment();
        await activityEnvironment.RunAsync(() => sut.SynchronizeAsync(input));

        subscription.StripeSubscriptionStatus.ShouldBeNull();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task CancelEntitlementSubscription(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceStripeSubscriptionCancellationIntegrations sut)
    {
        var purchase = new EntitlementPurchase
        {
            Id = "purchase-1",
            StripeAccountId = "acct-1",
            StripeSubscriptionId = null,
        };
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => purchaseRepository.GetByIdAsync(purchase.Id, A<CancellationToken>._)).Returns(purchase);
        var activityEnvironment = new ActivityEnvironment();
        await activityEnvironment.RunAsync(() => sut.SynchronizeAsync(
            new MarketplaceStripeSubscriptionCancellationInput("entitlement", purchase.Id, false)));

        purchase.StripeSubscriptionStatus.ShouldBe("canceled");
        A.CallTo(() => unitOfWork.SaveChangesAsync(activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
