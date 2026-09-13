using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceAutomaticPaymentStatusServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetForSubscriptionAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Map_The_Purchase_Specific_Stripe_Subscription(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository subscriptionRepository,
        MarketplaceAutomaticPaymentStatusService sut,
        CancellationToken cancellationToken)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1",
            StripeAccountId = "acct-1",
            StripeCustomerId = "cus-1",
            StripeSubscriptionId = "sub-1",
            StripePriceId = "price-1",
            StripeSubscriptionStatus = "past_due",
            StripeCurrentPeriodEndsAt = new DateTimeOffset(2026, 9, 12, 0, 0, 0, TimeSpan.Zero),
            StripeCancelAtPeriodEnd = true,
        };

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(subscriptionRepository);
        A.CallTo(() => subscriptionRepository.GetByIdAsync("subscription-1", cancellationToken))
            .Returns(subscription);

        var result = await sut.GetForSubscriptionAsync("subscription-1", cancellationToken);

        result.ShouldNotBeNull();
        result!.Configured.ShouldBeTrue();
        result.Status.ShouldBe("past_due");
        result.CurrentPeriodEndsAt.ShouldBe(subscription.StripeCurrentPeriodEndsAt);
        result.CancelAtPeriodEnd.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_The_Purchase_Has_No_Stripe_Subscription(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository subscriptionRepository,
        MarketplaceAutomaticPaymentStatusService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(subscriptionRepository);
        A.CallTo(() => subscriptionRepository.GetByIdAsync("subscription-1", cancellationToken))
            .Returns((MarketplaceBookingSubscription?)null);

        var result = await sut.GetForSubscriptionAsync("subscription-1", cancellationToken);

        result.ShouldBeNull();
    }
}
