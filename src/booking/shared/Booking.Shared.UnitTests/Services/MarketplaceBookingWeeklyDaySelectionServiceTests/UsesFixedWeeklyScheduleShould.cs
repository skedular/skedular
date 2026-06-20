using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingWeeklyDaySelectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UsesFixedWeeklyScheduleShould
{
    [Fact]
    public void Use_Selected_Weekly_Days_Instead_Of_The_Subscription_Start_Date()
    {
        var weeklyPricing = ProductPricing.Empty("weekly") with { PurchaseCadence = ProductPricingCadence.Weekly, RequiredDaysPerWeek = 2 };

        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(
                weeklyPricing,
                [DayOfWeek.Tuesday, DayOfWeek.Wednesday])
            .ShouldBeTrue();
        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(weeklyPricing, [])
            .ShouldBeFalse();
        MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(
                weeklyPricing with { PurchaseCadence = ProductPricingCadence.Monthly },
                [DayOfWeek.Tuesday, DayOfWeek.Wednesday])
            .ShouldBeFalse();
    }
}
