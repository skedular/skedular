using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundPolicyServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetRefundQuoteShould
{
    [Fact]
    public void Return_Non_Cancellable_When_Policy_Is_No_Cancellation()
    {
        var sut = new MarketplaceRefundPolicyService();
        var pricing = CreatePricing(ProductPricingCancellationPolicyType.NoCancellation, []);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 8, 0, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(false, false, 0, null));
    }

    [Fact]
    public void Return_Full_Refund_When_Full_Refund_Before_Cutoff_Has_No_Explicit_Rules_And_Request_Is_Before_Start()
    {
        var sut = new MarketplaceRefundPolicyService();
        var pricing = CreatePricing(ProductPricingCancellationPolicyType.FullRefundBeforeCutoff, []);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 8, 0, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(true, true, 100, 0));
        result.CalculateRefundAmount(123.45m).ShouldBe(123.45m);
    }

    [Fact]
    public void Return_Matching_Tiered_Rule_When_Request_Falls_Inside_A_Configured_Window()
    {
        var sut = new MarketplaceRefundPolicyService();
        var pricing = CreatePricing(
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(180, 100), new ProductPricingCancellationRefundRule(60, 50)]);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 10, 30, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(true, true, 50, 60));
        result.CalculateRefundAmount(200m).ShouldBe(100m);
    }

    [Fact]
    public void Return_Zero_Refund_But_Still_Cancellable_When_A_Zero_Percent_Rule_Matches()
    {
        var sut = new MarketplaceRefundPolicyService();
        var pricing = CreatePricing(
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(0, 0)]);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 11, 59, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(true, false, 0, 0));
    }

    [Fact]
    public void Return_Not_Cancellable_When_No_Rule_Applies()
    {
        var sut = new MarketplaceRefundPolicyService();
        var pricing = CreatePricing(
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(120, 100)]);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 10, 30, 1, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(false, false, 0, null));
    }

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new(
            "pricing-1",
            0,
            ListingMetadata.Empty,
            ProductPricingCadence.OneTime,
            ProductPricingCadence.OneTime,
            100m,
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
