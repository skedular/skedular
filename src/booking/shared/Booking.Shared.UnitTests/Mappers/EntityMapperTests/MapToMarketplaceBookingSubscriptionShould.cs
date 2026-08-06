using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.UnitTests.Mappers.EntityMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToMarketplaceBookingSubscriptionShould
{
    [Fact]
    public void Persist_Weekly_Selected_Days_Using_The_Stored_Day_Contract()
    {
        var sut = new EntityMapper(TimeProvider.System);
        var subscription = new MarketplaceBookingSubscription
        {
            WeeklySelectedDays = [DayOfWeek.Tuesday, DayOfWeek.Thursday],
        };

        var result = sut.MapTo(
            subscription,
            [],
            [],
            [],
            [],
            null,
            null,
            null,
            new MarketplaceBooking(),
            new ProductVersion());

        result.WeeklySelectedDays.ShouldBe([DayOfWeekConstants.Tuesday, DayOfWeekConstants.Thursday]);
    }
}
