using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.UnitTests.Mappers.EntityMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToMarketplaceBookingSubscriptionShould
{
    [Fact]
    public void Return_Default_Marketplace_Booking_When_Related_Data_Is_Missing()
    {
        var sut = new EntityMapper(TimeProvider.System);
        var subscription = new MarketplaceBookingSubscription
        {
            Status = MarketplaceBookingSubscriptionStatusConstants.Active,
            MarketplaceBooking = null!,
        };

        var result = sut.MapTo(subscription);

        result.MarketplaceBooking.ShouldNotBeNull();
        result.MarketplaceBooking.ProductVersion.ShouldNotBeNull();
        result.InvolvedCustomers.ShouldBeEmpty();
        result.RecurringBookings.ShouldBeEmpty();
    }

    [Fact]
    public void Persist_Weekly_Selected_Days_Using_The_Stored_Day_Contract()
    {
        var sut = new EntityMapper(TimeProvider.System);
        var subscription = new Shared.Models.MarketplaceBookingSubscription
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
