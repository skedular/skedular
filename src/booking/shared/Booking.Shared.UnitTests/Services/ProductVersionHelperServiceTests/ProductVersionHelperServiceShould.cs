using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.ProductVersionHelperServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProductVersionHelperServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_Returns_Pricing_With_Same_Id(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront
        };
        var pricingOptions = new[]
        {
            ProductPricing.Empty("pricing-2") with
            {
                PurchaseCadence = ProductPricingCadence.Daily,
                BookingCadence = ProductPricingCadence.Daily,
                NumberOfResourcesToBook = 1,
                BillingMode = ProductPricingBillingMode.Upfront
            },
            pricing
        };

        var result = sut.FindMatchingPricing(pricingOptions, pricing);

        result.ShouldBe(pricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_Returns_Pricing_With_Matching_Properties_When_Id_Not_Found(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront
        };
        var matchingPricing = ProductPricing.Empty("pricing-2") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront
        };
        var pricingOptions = new[] { matchingPricing };

        var result = sut.FindMatchingPricing(pricingOptions, pricing);

        result.ShouldBe(matchingPricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_Ignores_Available_Days_When_Using_The_Fallback_Identity(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront,
            AvailableDays = [DayOfWeek.Sunday]
        };
        var matchingPricing = pricing with { Id = "pricing-2", AvailableDays = [DayOfWeek.Saturday] };

        var result = sut.FindMatchingPricing([matchingPricing], pricing);

        result.ShouldBe(matchingPricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_Uses_Required_Days_Per_Week_When_Using_The_Fallback_Identity(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-2-days") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            BookingCadence = ProductPricingCadence.Weekly,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront,
            RequiredDaysPerWeek = 2
        };
        var nonMatchingPricing = pricing with { Id = "pricing-3-days", RequiredDaysPerWeek = 3 };
        var matchingPricing = pricing with { Id = "pricing-2-days-current" };

        var result = sut.FindMatchingPricing([nonMatchingPricing, matchingPricing], pricing);

        result.ShouldBe(matchingPricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_Returns_Null_When_No_Match_Found(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront
        };
        var pricingOptions = new[]
        {
            ProductPricing.Empty("pricing-2") with
            {
                PurchaseCadence = ProductPricingCadence.PerHour,
                BookingCadence = ProductPricingCadence.PerHour,
                NumberOfResourcesToBook = 2,
                BillingMode = ProductPricingBillingMode.InArrears
            }
        };

        var result = sut.FindMatchingPricing(pricingOptions, pricing);

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_For_StripeProduct_Returns_Product_With_Same_Product_Pricing_Id(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1, BillingMode = ProductPricingBillingMode.Upfront
        };
        var stripeProduct = new StripeProduct
        {
            Id = "stripe-product-1",
            ProductPricingId = "pricing-1",
            PricingCadence = ProductPricingCadence.Daily.ToProductPricingCadence(),
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode()
        };
        var stripeProducts = new[] { stripeProduct };

        var result = sut.FindMatchingPricing(stripeProducts, pricing);

        result.ShouldBe(stripeProduct);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_For_StripeProduct_Returns_Null_When_Product_Pricing_Id_Does_Not_Match(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1, BillingMode = ProductPricingBillingMode.Upfront
        };
        var matchingStripeProduct = new StripeProduct
        {
            Id = "stripe-product-2",
            ProductPricingId = "pricing-2",
            PricingCadence = ProductPricingCadence.Daily.ToProductPricingCadence(),
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode()
        };
        var stripeProducts = new[] { matchingStripeProduct };

        var result = sut.FindMatchingPricing(stripeProducts, pricing);

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_For_StripeProduct_Distinguishes_Weekly_Required_Day_Tiers(
        ProductVersionHelperService sut)
    {
        var twoDayPricing = ProductPricing.Empty("pricing-2-days") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            NumberOfResourcesToBook = 1,
            BillingMode = ProductPricingBillingMode.Upfront,
            RequiredDaysPerWeek = 2
        };
        var threeDayPricing = twoDayPricing with { Id = "pricing-3-days", RequiredDaysPerWeek = 3 };
        var stripeProducts = new[]
        {
            new StripeProduct { Id = "stripe-product-2-days", ProductPricingId = twoDayPricing.Id },
            new StripeProduct { Id = "stripe-product-3-days", ProductPricingId = threeDayPricing.Id }
        };

        var result = sut.FindMatchingPricing(stripeProducts, threeDayPricing);

        result.ShouldBe(stripeProducts[1]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void FindMatchingPricing_For_StripeProduct_Returns_Null_When_No_Match_Found(
        ProductVersionHelperService sut)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1, BillingMode = ProductPricingBillingMode.Upfront
        };
        var stripeProducts = new[]
        {
            new StripeProduct
            {
                Id = "pricing-2",
                PricingCadence = ProductPricingCadence.PerHour.ToProductPricingCadence(),
                NumberOfResourcesToBook = 2,
                BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode()
            }
        };

        var result = sut.FindMatchingPricing(stripeProducts, pricing);

        result.ShouldBeNull();
    }
}
