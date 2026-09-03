using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingWeeklyDaySelectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UsesFixedWeeklyScheduleShould
{
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
    public void Use_Selected_Days_For_A_Supported_Calendar_Cadence(ProductPricingCadence cadence)
    {
        var weeklyPricing = ProductPricing.Empty("weekly") with
        {
            PurchaseCadence = cadence,
            RequiredDaysPerWeek = 2,
        };

        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(
                weeklyPricing,
                [DayOfWeek.Tuesday, DayOfWeek.Wednesday])
            .ShouldBeTrue();
        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(weeklyPricing, [])
            .ShouldBeFalse();
        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(
                weeklyPricing with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                },
                [DayOfWeek.Tuesday, DayOfWeek.Wednesday])
            .ShouldBeFalse();
    }
}
