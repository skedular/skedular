using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.GraphQL.RecurringBooking;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.MarketplaceBookingSubscriptionDetailsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingInstancesShould
{
    [Fact]
    public void Return_A_Paginated_Connection_Of_Instances()
    {
        var subscription = new MarketplaceBookingSubscriptionDetails
        {
            RecurringBookings = Enumerable.Range(1, 3)
                .Select(index => new RecurringBookingDetails
                {
                    Id = $"recurring-{index}",
                    StartDate = new DateTimeOffset(2026, 1, index, 0, 0, 0, TimeSpan.Zero),
                    EndDate = null,
                }),
        };

        var firstPage = subscription.GetBookingInstances(null, 2, null, null, null, null);
        var secondPage = subscription.GetBookingInstances(firstPage.PageInfo.EndCursor, 2, null, null, null, null);

        firstPage.TotalCount.ShouldBe(3);
        firstPage.Edges.Count().ShouldBe(2);
        firstPage.PageInfo.HasNextPage.ShouldBeTrue();
        firstPage.Edges.First().Node.MarketplaceBookingSubscriptionId.ShouldBeNull();
        secondPage.Edges.Single().Node.Id.ShouldBe("recurring-3");
        secondPage.PageInfo.HasPreviousPage.ShouldBeTrue();
    }

    [Fact]
    public void Filter_Instances_By_Date_Window()
    {
        var subscription = new MarketplaceBookingSubscriptionDetails
        {
            RecurringBookings =
            [
                new RecurringBookingDetails
                {
                    Id = "before",
                    StartDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                },
                new RecurringBookingDetails
                {
                    Id = "inside",
                    StartDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
                },
                new RecurringBookingDetails
                {
                    Id = "after",
                    StartDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
                },
            ],
        };

        var result = subscription.GetBookingInstances(
            null,
            null,
            null,
            null,
            new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 20, 0, 0, 0, TimeSpan.Zero));

        result.Edges.Single().Node.Id.ShouldBe("inside");
    }
}
