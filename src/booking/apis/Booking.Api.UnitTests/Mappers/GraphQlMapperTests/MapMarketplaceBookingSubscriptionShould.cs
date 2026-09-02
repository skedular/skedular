using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Mappers;
using Booking.Shared.Models;

namespace Booking.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapMarketplaceBookingSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Weekly_Selected_Days(GraphQlMapper sut)
    {
        var input = new AddMarketplaceBookingSubscriptionInput
        {
            WeeklySelectedDays = [DayOfWeek.Tuesday, DayOfWeek.Thursday],
        };

        var subscription = sut.MapTo(input);

        subscription.WeeklySelectedDays.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Thursday]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Selected_Booking_End_Time(GraphQlMapper sut)
    {
        var until = new TimeOnly(10, 0);
        var input = new AddMarketplaceBookingSubscriptionInput
        {
            StartedAt = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero),
            From = new TimeOnly(9, 0),
            Until = until,
        };

        var subscription = sut.MapTo(input);

        subscription.Until.ShouldBe(until);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_The_Latest_Billing_Period_Amount_For_The_Subscription_Header(GraphQlMapper sut)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Status = MarketplaceBookingSubscriptionStatus.Cancelled,
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductPricing = ProductPricing.Empty("subscription-pricing") with
                {
                    Price = 100m,
                },
            },
            RecurringBookings =
            [
                new RecurringBooking
                {
                    StartDate = new DateTimeOffset(2026, 8, 31, 0, 0, 0, TimeSpan.Zero),
                    MarketplaceBooking = new MarketplaceBooking
                    {
                        TotalAmount = 100m,
                    },
                },
                new RecurringBooking
                {
                    StartDate = new DateTimeOffset(2026, 9, 7, 0, 0, 0, TimeSpan.Zero),
                    MarketplaceBooking = new MarketplaceBooking
                    {
                        TotalAmount = 125m,
                    },
                },
            ],
        };

        var result = sut.MapTo(subscription);

        result.MarketplaceBooking.TotalAmount.ShouldBe(125m);
    }
}
