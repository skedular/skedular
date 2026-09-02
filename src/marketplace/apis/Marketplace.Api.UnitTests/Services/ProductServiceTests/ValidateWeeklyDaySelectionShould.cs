using Api.Shared.Services;
using Api.Shared.Services.Models;
using Marketplace.Api.Services;

namespace Marketplace.Api.UnitTests.Services.ProductServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidateWeeklyDaySelectionShould
{
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void Reject_Invalid_Required_Weekly_Day_Count(int requiredDaysPerWeek)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            AvailableDays = [DayOfWeek.Monday, DayOfWeek.Tuesday],
            RequiredDaysPerWeek = requiredDaysPerWeek,
        };

        Should.Throw<ProductPricingWeeklyDaySelectionRangeInvalid>(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }

    [Theory]
    [InlineData(ProductPricingCadence.Weekly)]
    [InlineData(ProductPricingCadence.Fortnightly)]
    [InlineData(ProductPricingCadence.Monthly)]
    [InlineData(ProductPricingCadence.TwoMonths)]
    [InlineData(ProductPricingCadence.Quarterly)]
    [InlineData(ProductPricingCadence.FourMonths)]
    [InlineData(ProductPricingCadence.FiveMonths)]
    [InlineData(ProductPricingCadence.SixMonths)]
    [InlineData(ProductPricingCadence.Yearly)]
    public void Accept_Required_Days_For_Supported_Calendar_Price(ProductPricingCadence cadence)
    {
        var pricing = ProductPricing.Empty("monthly") with
        {
            PurchaseCadence = cadence,
            RequiredDaysPerWeek = 1,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            BillingMode = ProductPricingBillingMode.Upfront,
            CancellationPolicyType = ProductPricingCancellationPolicyType.NoCancellation,
        };

        Should.NotThrow(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }

    [Fact]
    public void Reject_Required_Days_For_Daily_Price()
    {
        var pricing = ProductPricing.Empty("daily") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            RequiredDaysPerWeek = 1,
        };

        Should.Throw<ProductPricingWeeklyDaySelectionOnlySupportedForWeeklyPricing>(() =>
            ProductService.Validate(ProductType.Resource, pricing, false));
    }

    [Fact]
    public void Allow_Multiple_Entitlement_Redemptions_On_One_Available_Day()
    {
        var pricing = ProductPricing.Empty("entitlement") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            PurchaseCadence = ProductPricingCadence.NotSet,
            AvailableDays = [DayOfWeek.Monday],
            RequiredDaysPerWeek = 2,
            AcceptedPaymentMethods = [PaymentMethod.Card],
            BillingMode = ProductPricingBillingMode.Upfront,
            CancellationPolicyType = ProductPricingCancellationPolicyType.NoCancellation,
        };

        Should.NotThrow(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }
}
