using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingWeeklyDaySelectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UsesFixedWeeklyScheduleShould
{
    [Theory]
    [InlineData(MembershipTerm.Weekly)]
    [InlineData(MembershipTerm.Fortnightly)]
    [InlineData(MembershipTerm.Monthly)]
    [InlineData(MembershipTerm.TwoMonths)]
    [InlineData(MembershipTerm.Quarterly)]
    [InlineData(MembershipTerm.FourMonths)]
    [InlineData(MembershipTerm.FiveMonths)]
    [InlineData(MembershipTerm.SixMonths)]
    [InlineData(MembershipTerm.Yearly)]
    public void Use_Selected_Days_For_A_Supported_Calendar_Cadence(MembershipTerm cadence)
    {
        var weeklyPricing = ProductPricing.Empty("weekly") with
        {
            MembershipTerm = cadence,
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
                    MembershipTerm = MembershipTerm.Daily,
                },
                [DayOfWeek.Tuesday, DayOfWeek.Wednesday])
            .ShouldBeFalse();
    }
}
