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
            RequiredDaysPerWeek = requiredDaysPerWeek
        };

        Should.Throw<ProductPricingWeeklyDaySelectionRangeInvalid>(() => ProductService.Validate(ProductType.Resource, pricing, false));
    }

    [Fact]
    public void Reject_Weekly_Bounds_For_A_NonWeekly_Price()
    {
        var pricing = ProductPricing.Empty("monthly") with { PurchaseCadence = ProductPricingCadence.Monthly, RequiredDaysPerWeek = 1 };

        Should.Throw<ProductPricingWeeklyDaySelectionOnlySupportedForWeeklyPricing>(() =>
            ProductService.Validate(ProductType.Resource, pricing, false));
    }
}
