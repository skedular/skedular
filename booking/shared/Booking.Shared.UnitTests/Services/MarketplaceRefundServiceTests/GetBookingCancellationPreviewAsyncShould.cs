using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetBookingCancellationPreviewAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Refundable_Preview_For_One_Time_Booking(
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking(
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(180, 50)]);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);

        var result = await sut.GetBookingCancellationPreviewAsync(booking, cancellationToken);

        result.OrganizationId.ShouldBe("org-1");
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBooking);
        result.LocalEntityId.ShouldBe("marketplace-booking-1");
        result.RequestedAt.ShouldBe(requestedAt);
        result.ReferenceTime.ShouldBe(booking.From);
        result.IsRefundable.ShouldBeTrue();
        result.RefundPercentage.ShouldBe(50);
        result.AppliedRuleMinutesBefore.ShouldBe(180);
        result.BaseAmount.ShouldBe(120m);
        result.RefundAmount.ShouldBe(60m);
        result.Currency.ShouldBe("NZD");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Non_Refundable_Preview_When_Payment_Is_Not_Confirmed(
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking(
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            []);
        booking.MarketplaceBooking!.PaymentStatus = PaymentStatus.Pending.ToPaymentStatus();

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);

        var result = await sut.GetBookingCancellationPreviewAsync(booking, cancellationToken);

        result.IsRefundable.ShouldBeFalse();
        result.RefundPercentage.ShouldBe(0);
        result.BaseAmount.ShouldBeNull();
        result.RefundAmount.ShouldBeNull();
    }

    private static Database.Entities.Booking CreateBooking(
        DateTimeOffset from,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "booking-1",
            From = from,
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = "marketplace-booking-1",
                Quantity = 1,
                TotalAmount = 120m,
                Currency = "NZD",
                PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
                ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules),
                ProductVersion = new ProductVersionEntity { Product = new ProductEntity { Organization = new OrganizationEntity { Id = "org-1" } } }
            }
        };

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new(
            "pricing-1",
            0,
            ListingMetadata.Empty,
            ProductPricingCadence.OneTime,
            ProductPricingCadence.OneTime,
            120m,
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
