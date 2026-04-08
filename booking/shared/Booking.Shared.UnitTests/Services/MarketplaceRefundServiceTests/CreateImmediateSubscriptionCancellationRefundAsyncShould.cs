using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreateImmediateSubscriptionCancellationRefundAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Existing_Refund_Record_When_Subscription_Refund_Already_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var requestedByCustomer = new CustomerEntity { Id = "customer-1" };
        var subscription = CreateSubscription(
            requestedAt.AddDays(-5),
            requestedAt.AddDays(2),
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(1440, 75), new ProductPricingCancellationRefundRule(60, 25)]);
        var existingRefund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = subscription.Id,
            Status = "OldStatus"
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                "org-1",
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                subscription.Id,
                cancellationToken))
            .Returns(existingRefund);
        A.CallTo(() => marketplaceRefundRepository.Update(existingRefund))
            .Returns(existingRefund);

        var result = await sut.CreateImmediateSubscriptionCancellationRefundAsync(subscription, requestedByCustomer, cancellationToken);

        result.ShouldBe(existingRefund);
        existingRefund.Status.ShouldBe(MarketplaceRefundStatusConstants.Requested);
        existingRefund.RequestedAt.ShouldBe(requestedAt);
        existingRefund.ReferenceTime.ShouldBe(subscription.NextRenewalAt!.Value);
        existingRefund.RefundPercentage.ShouldBe(75);
        existingRefund.AppliedRuleMinutesBefore.ShouldBe(1440);
        existingRefund.BaseAmount.ShouldBe(80m);
        existingRefund.RefundAmount.ShouldBe(60m);
        existingRefund.Currency.ShouldBe("NZD");
        existingRefund.RequestedByCustomer.ShouldBe(requestedByCustomer);
        A.CallTo(() => marketplaceRefundRepository.Update(existingRefund)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Current_Billing_Window_Payment_Is_Not_Confirmed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var subscription = CreateSubscription(
            requestedAt.AddDays(-5),
            requestedAt.AddDays(2),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            []);
        subscription.RecurringBookings =
        [
            new RecurringBookingEntity
            {
                Id = "recurring-1",
                StartDate = requestedAt.AddDays(-5),
                MarketplaceBooking = new MarketplaceBookingEntity
                {
                    PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(), ProductPricing = subscription.MarketplaceBooking.ProductPricing
                }
            }
        ];

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);

        var result = await sut.CreateImmediateSubscriptionCancellationRefundAsync(subscription, null, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    private static MarketplaceBookingSubscriptionEntity CreateSubscription(
        DateTimeOffset startedAt,
        DateTimeOffset? nextRenewalAt,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "subscription-1",
            StartedAt = startedAt,
            NextRenewalAt = nextRenewalAt,
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = "marketplace-booking-1",
                Quantity = 1,
                TotalAmount = 80m,
                Currency = "NZD",
                PaymentStatus = PaymentStatus.NotSet.ToPaymentStatus(),
                ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules),
                ProductVersion = new ProductVersionEntity
                {
                    Product = new ProductEntity
                    {
                        Organization = new OrganizationEntity { Id = "org-1", BillingCycle = OrganizationBillingCycleConstants.Monthly }
                    }
                }
            },
            RecurringBookings =
            [
                new RecurringBookingEntity
                {
                    Id = "recurring-1",
                    StartDate = startedAt,
                    MarketplaceBooking = new MarketplaceBookingEntity
                    {
                        PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
                        ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules)
                    }
                }
            ]
        };

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new(
            "pricing-1",
            0,
            ListingMetadata.Empty,
            ProductPricingCadence.Monthly,
            ProductPricingCadence.Monthly,
            80m,
            true,
            false,
            [],
            ProductPricingBillingMode.Upfront,
            null,
            null,
            10,
            10,
            1,
            cancellationPolicyType,
            cancellationRefundRules);
}
