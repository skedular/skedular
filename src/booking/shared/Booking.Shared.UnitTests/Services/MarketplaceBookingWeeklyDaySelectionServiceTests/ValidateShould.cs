using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingWeeklyDaySelectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ValidateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Accept_A_Valid_Weekly_Selection(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            AvailableDays = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday],
            RequiredDaysPerWeek = 2,
        };

        var result = sut.Validate(pricing, [DayOfWeek.Monday, DayOfWeek.Wednesday]);

        result.ShouldBe([DayOfWeek.Monday, DayOfWeek.Wednesday]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_A_Duplicate_Selection(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            RequiredDaysPerWeek = 1,
        };

        Should.Throw<MarketplaceBookingWeeklyDaySelectionInvalid>(() =>
            sut.Validate(pricing, [DayOfWeek.Tuesday, DayOfWeek.Tuesday]));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_A_Day_Outside_Available_Days(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            AvailableDays = [DayOfWeek.Tuesday],
            RequiredDaysPerWeek = 1,
        };

        Should.Throw<MarketplaceBookingWeeklyDaySelectionInvalid>(() =>
            sut.Validate(pricing, [DayOfWeek.Wednesday]));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Accept_A_Day_When_Available_Days_Are_Not_Restricted(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            AvailableDays = [],
            RequiredDaysPerWeek = 1,
        };

        var result = sut.Validate(pricing, [DayOfWeek.Sunday]);

        result.ShouldBe([DayOfWeek.Sunday]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_A_Selection_That_Does_Not_Match_The_Required_Count(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            RequiredDaysPerWeek = 2,
        };

        Should.Throw<MarketplaceBookingWeeklyDaySelectionInvalid>(() => sut.Validate(pricing, [DayOfWeek.Tuesday]));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_A_Selection_For_A_NonWeekly_Price(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("daily") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            RequiredDaysPerWeek = 1,
        };

        Should.Throw<MarketplaceBookingWeeklyDaySelectionInvalid>(() => sut.Validate(pricing, [DayOfWeek.Tuesday]));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Selection_When_The_Weekly_Rule_Is_Not_Configured(MarketplaceBookingWeeklyDaySelectionService sut)
    {
        var pricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
        };

        var result = sut.Validate(pricing, [DayOfWeek.Monday]);

        result.ShouldBeEmpty();
    }
}
