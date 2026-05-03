using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetImmediateSubscriptionCancellationPreviewAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Next_Renewal_At_As_Subscription_Reference_Time(
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var subscription = CreateSubscription(
            requestedAt.AddDays(-5),
            requestedAt.AddDays(2),
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(1440, 75), new ProductPricingCancellationRefundRule(60, 25)]);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);

        var result = await sut.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken);

        result.OrganizationId.ShouldBe("org-1");
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription);
        result.LocalEntityId.ShouldBe(subscription.Id);
        result.ReferenceTime.ShouldBe(subscription.NextRenewalAt!.Value);
        result.IsRefundable.ShouldBeTrue();
        result.RefundPercentage.ShouldBe(75);
        result.RefundAmount.ShouldBe(60m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Non_Refundable_Preview_When_Current_Billing_Window_Payment_Is_Not_Confirmed(
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

        var result = await sut.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken);

        result.IsRefundable.ShouldBeFalse();
        result.RefundPercentage.ShouldBe(0);
        result.BaseAmount.ShouldBeNull();
        result.RefundAmount.ShouldBeNull();
    }

    private static MarketplaceBookingSubscriptionEntity CreateSubscription(
        DateTimeOffset startedAt,
        DateTimeOffset? nextRenewalAt,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
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
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
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
