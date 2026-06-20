using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingAvailableDaysServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsAvailableShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Treat_Empty_Available_Days_As_Every_Day(MarketplaceBookingAvailableDaysService sut)
    {
        var pricing = ProductPricing.Empty("price");

        var available = sut.IsAvailable(
            pricing,
            new DateTimeOffset(2026, 7, 17, 20, 0, 0, TimeSpan.Zero),
            out var bookingDate);

        available.ShouldBeTrue();
        bookingDate.ShouldBe(new DateOnly(2026, 7, 17));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_The_Booking_Calendar_Day(MarketplaceBookingAvailableDaysService sut)
    {
        var pricing = ProductPricing.Empty("price") with { AvailableDays = [DayOfWeek.Friday] };

        var available = sut.IsAvailable(
            pricing,
            new DateTimeOffset(2026, 7, 17, 20, 0, 0, TimeSpan.Zero),
            out var bookingDate);

        available.ShouldBeTrue();
        bookingDate.ShouldBe(new DateOnly(2026, 7, 17));
    }
}
