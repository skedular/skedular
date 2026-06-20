using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Mappers;

namespace Booking.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapMarketplaceBookingSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Weekly_Selected_Days(GraphQlMapper sut)
    {
        var input = new AddMarketplaceBookingSubscriptionInput { WeeklySelectedDays = [DayOfWeek.Tuesday, DayOfWeek.Thursday] };

        var subscription = sut.MapTo(input);

        subscription.WeeklySelectedDays.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Thursday]);
    }
}
