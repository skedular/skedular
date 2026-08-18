using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundPolicyServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetRefundQuoteShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Non_Cancellable_When_Policy_Is_No_Cancellation(MarketplaceRefundPolicyService sut)
    {
        var pricing = CreatePricing(ProductPricingCancellationPolicyType.NoCancellation, []);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 8, 0, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(false, false, 0, null));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Full_Refund_When_Full_Refund_Before_Cutoff_Has_No_Explicit_Rules_And_Request_Is_Before_Start(
        MarketplaceRefundPolicyService sut)
    {
        var pricing = CreatePricing(ProductPricingCancellationPolicyType.FullRefundBeforeCutoff, []);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 8, 0, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(true, true, 100, 0));
        result.CalculateRefundAmount(123.45m).ShouldBe(123.45m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Matching_Tiered_Rule_When_Request_Falls_Inside_A_Configured_Window(MarketplaceRefundPolicyService sut)
    {
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

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Zero_Refund_But_Still_Cancellable_When_A_Zero_Percent_Rule_Matches(MarketplaceRefundPolicyService sut)
    {
        var pricing = CreatePricing(
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(0, 0)]);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 11, 59, 0, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(true, false, 0, 0));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Not_Cancellable_When_No_Rule_Applies(MarketplaceRefundPolicyService sut)
    {
        var pricing = CreatePricing(
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(120, 100)]);

        var result = sut.GetQuote(
            pricing,
            new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 10, 10, 30, 1, TimeSpan.Zero));

        result.ShouldBe(new MarketplaceRefundQuote(false, false, 0, null));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Apply_A_Rule_At_The_Exact_Cutoff(MarketplaceRefundPolicyService sut)
    {
        var referenceTime = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero);
        var result = sut.GetQuote(
            CreatePricing(ProductPricingCancellationPolicyType.TieredRefund,
                [new ProductPricingCancellationRefundRule(60, 75)]),
            referenceTime,
            referenceTime.AddMinutes(-60));

        result.ShouldBe(new MarketplaceRefundQuote(true, true, 75, 60));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Select_The_Most_Generous_Applicable_Longer_Window(MarketplaceRefundPolicyService sut)
    {
        var referenceTime = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero);
        var result = sut.GetQuote(
            CreatePricing(ProductPricingCancellationPolicyType.TieredRefund,
                [new ProductPricingCancellationRefundRule(60, 50), new ProductPricingCancellationRefundRule(180, 100)]),
            referenceTime,
            referenceTime.AddMinutes(-240));

        result.RefundPercentage.ShouldBe(100);
        result.AppliedRuleMinutesBefore.ShouldBe(180);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Clamp_Refund_Percentage_To_The_Valid_Monetary_Range(MarketplaceRefundPolicyService sut)
    {
        var referenceTime = new DateTimeOffset(2026, 4, 10, 12, 0, 0, TimeSpan.Zero);
        var pricing = CreatePricing(ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(0, 150)]);

        sut.GetQuote(pricing, referenceTime, referenceTime)
            .RefundPercentage.ShouldBe(100);
    }

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
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
